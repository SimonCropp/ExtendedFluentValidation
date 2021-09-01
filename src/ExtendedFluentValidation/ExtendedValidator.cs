using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace FluentValidation
{
    public class ExtendedValidator<T> :
        AbstractValidator<T>
    {
        static List<IValidator> sharedValidators;

        static ExtendedValidator()
        {
            sharedValidators = ValidatorExtensions.GetSharedValidatorsFor<T>().ToList();
        }

        public ExtendedValidator()
        {
            this.AddExtendedRules();
        }

        public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property)
        {
            return this.RuleFor<T, TProperty>(property);
        }

        public override ValidationResult Validate(ValidationContext<T> context)
        {
            var result = base.Validate(context);
            if (!sharedValidators.Any())
            {
                return result;
            }

            var innerErrors = sharedValidators.SelectMany(x => x.Validate(context.Clone()).Errors);
            return MergeErrors(innerErrors, result.Errors);
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default)
        {
            var result = await base.ValidateAsync(context, cancellation);
            if (!sharedValidators.Any())
            {
                return result;
            }
            
            var innerErrors = new List<ValidationFailure>();
            foreach (var innerValidator in sharedValidators)
            {
                var validationResult = await innerValidator.ValidateAsync(context.Clone(), cancellation);
                innerErrors.AddRange(validationResult.Errors);
            }
            return MergeErrors(innerErrors, result.Errors);
        }

        static ValidationResult MergeErrors(IEnumerable<ValidationFailure> innerErrors, List<ValidationFailure> errors)
        {
            foreach (var innerError in innerErrors)
            {
                if (errors.Any(x => x.ErrorMessage == innerError.ErrorMessage))
                {
                    continue;
                }

                errors.Add(innerError);
            }

            return new(errors);
        }

    }
}