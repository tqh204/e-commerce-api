using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.productId)
                .NotEmpty().WithMessage("Id sản phẩm không hợp lệ");

            RuleFor(x => x.productName)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự");

            RuleFor(x => x.productDescription)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.productDescription));

            RuleFor(x => x.price)
                .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0");

            RuleFor(x => x.stockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được âm");

            RuleFor(x => x.categoryId)
                .GreaterThan(0).WithMessage("CategoryId phải hợp lệ");
        }
    }
}
