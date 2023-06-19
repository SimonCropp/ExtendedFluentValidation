using FluentValidation.Results;

namespace FluentValidation;

public static class ExtendedValidator
{
    public static IValidator GetInstance(Type type)
    {
        var genericType = typeof(ExtendedValidator<>).MakeGenericType(type);
        return (IValidator)Activator.CreateInstance(genericType)!;
    }
}

public class ExtendedValidator<T> :
    AbstractValidator<T>
{
    static List<IValidator> sharedValidators;

    static ExtendedValidator() =>
        sharedValidators = ValidatorExtensions.GetSharedValidatorsFor<T>().ToList();

    public ExtendedValidator() =>
        this.AddExtendedRules();

    public ExtendedValidator(params string[] exclusions) =>
        this.AddExtendedRules(exclusions);

    public ExtendedValidator(IReadOnlyList<string>? exclusions = null, bool validateEmptyLists = false) =>
        this.AddExtendedRules(exclusions, validateEmptyLists);

    public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property) =>
        this.RuleFor<T, TProperty>(property);

    public override ValidationResult Validate(ValidationContext<T> context)
    {
        var result = base.Validate(context);
        if (!sharedValidators.Any())
        {
            return result;
        }

        var inner = sharedValidators.SelectMany(_ => _.Validate(context.Clone()).Errors);
        return MergeErrors(inner, result.Errors);
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, Cancel cancel = default)
    {
        var result = await base.ValidateAsync(context, cancel);
        if (!sharedValidators.Any())
        {
            return result;
        }

        var inner = new List<ValidationFailure>();
        foreach (var innerValidator in sharedValidators)
        {
            var validationResult = await innerValidator.ValidateAsync(context.Clone(), cancel);
            inner.AddRange(validationResult.Errors);
        }

        return MergeErrors(inner, result.Errors);
    }

    static ValidationResult MergeErrors(IEnumerable<ValidationFailure> innerErrors, List<ValidationFailure> errors)
    {
        foreach (var innerError in innerErrors)
        {
            if (errors.Any(_ => _.ErrorMessage == innerError.ErrorMessage))
            {
                continue;
            }

            errors.Add(innerError);
        }

        return new(errors);
    }

}