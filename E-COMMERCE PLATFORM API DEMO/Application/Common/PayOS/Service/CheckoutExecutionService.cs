using Application.Common.Results;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.PayOS.Service
{
    public class CheckoutExecutionService : ICheckoutExecutionService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutExecutionService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ICouponRepository couponRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> ExecuteAsync(
            Guid userId,
            CheckoutSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var products = new Dictionary<Guid, Product>();

                foreach (var item in snapshot.items)
                {
                    var product = await _productRepository.GetProductByIdAsync(item.productId);
                    if (product == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<Guid>.Failure($"Không tìm thấy sản phẩm {item.productName}");
                    }

                    if (product.stockQuantity < item.quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<Guid>.Failure($"Sản phẩm {product.productName} không đủ hàng");
                    }

                    products[item.productId] = product;
                }

                var order = new Order
                {
                    orderId = Guid.NewGuid(),
                    userId = userId,
                    totalAmount = snapshot.totalAmount,
                    status = OrderStatus.Pending,
                    createdAt = DateTime.UtcNow,
                    updatedAt = null
                };

                foreach (var item in snapshot.items)
                {
                    var product = products[item.productId];
                    product.stockQuantity -= item.quantity;
                    product.updatedAt = DateTime.UtcNow;
                    _productRepository.Update(product);

                    order.items.Add(new OrderItem
                    {
                        orderItemId = Guid.NewGuid(),
                        orderId = order.orderId,
                        productId = item.productId,
                        productName = item.productName,
                        quantity = item.quantity,
                        unitPrice = item.unitPrice,
                        lineTotal = item.lineTotal
                    });
                }

                if (snapshot.couponId.HasValue)
                {
                    var coupon = await _couponRepository.GetCouponByIdAsync(snapshot.couponId.Value);
                    if (coupon != null)
                    {
                        coupon.usedCount += 1;
                        coupon.updatedAt = DateTime.UtcNow;
                        _couponRepository.Update(coupon);
                    }
                }

                await _orderRepository.AddAsync(order);

                var currentCart = await _cartRepository.GetByUserIdAsync(userId);
                if (currentCart != null)
                {
                    _cartRepository.RemoveCart(currentCart);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result<Guid>.Success(order.orderId);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
