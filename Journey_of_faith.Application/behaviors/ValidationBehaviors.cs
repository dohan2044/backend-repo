using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.behaviors
{
    public class ValidationBehaviors<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token)
        {
            // đấy ra ngoại lệ nếu tham số là null
            ArgumentNullException.ThrowIfNull(request);

            if(validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, token))).ConfigureAwait(false);


                var faiures = validationResults
                    .Where(e => e.Errors.Count > 0)
                    .SelectMany(e => e.Errors)
                    .ToList();


                if(faiures.Count > 0)
                {
                    throw new FluentValidation.ValidationException(faiures);
                }
            }

            return await next().ConfigureAwait(false);
        }
    }
}
