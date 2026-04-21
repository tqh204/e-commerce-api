using Domain.Entities;
using MediatR;

namespace Application.Features.Promotion.Commands
{
    public record DeletePromotionRuleCommand(Guid ruleId) : IRequest<Result<bool>>;
}
