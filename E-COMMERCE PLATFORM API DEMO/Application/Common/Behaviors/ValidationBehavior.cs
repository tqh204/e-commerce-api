using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }
            return await next();
        }
    }
}
//This is the core of validation. When a request is sent to the mediator, it will first go through this pipeline behavior.
//It will check if there are any validators for the request. If there are, it will validate the request using all the validators.
//If any of the validators fail, it will throw a ValidationException with the details of the failures.
//If all validators pass, it will call the next delegate in the pipeline, which is usually the request handler.