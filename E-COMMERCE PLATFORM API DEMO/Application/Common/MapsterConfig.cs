using Application.Common.Loyalty;
using Application.Features.Order.Queries;
using Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public static class MapsterConfig
    {
        public static void Register()
        {
            TypeAdapterConfig<OrderItem, OrderItemDTO>
                .NewConfig()
                .Map(dest => dest.productName, src => src.productName ?? "");

            TypeAdapterConfig<Order, OrderDTO>
                .NewConfig()
                .Map(dest => dest.orderId, src => src.orderId)
                .Map(dest => dest.status, src => src.status.ToString());

            TypeAdapterConfig<User, LoyaltySummaryDTO>
                .NewConfig()
                .Map(dest => dest.userId, src => src.Id)
                .Map(dest => dest.loyaltyPoints, src => src.loyaltyPoint)
                .Map(dest => dest.rank, src => src.rank.ToString());
        }
    }
}
