namespace FluentValidation;

public static class ExtendedValidator
{
    public static IValidator GetInstance([DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] Type type)
    {
        var genericType = typeof(ExtendedValidator<>).MakeGenericType(type);
        return (IValidator)Activator.CreateInstance(genericType)!;
    }
}

public class ExtendedValidator<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T> :
    AbstractValidator<T>
{
    static List<IValidator> sharedValidators;

    RuleBuilder<T> ruleBuilder;

    static ExtendedValidator() =>
        sharedValidators = ValidatorConventions.GetValidatorsFor<T>().ToList();

    public ExtendedValidator() :
        this(null, (bool?)null)
    {
    }

    public ExtendedValidator(params string[] exclusions) :
        this((IReadOnlyList<string>)exclusions)
    {
    }

    public ExtendedValidator(IReadOnlyList<string>? exclusions = null, bool? validateEmptyLists = null) =>
        ruleBuilder = new(this, exclusions, validateEmptyLists);

    public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property) =>
        ruleBuilder.RuleFor<TProperty>(property);

    public override ValidationResult Validate(ValidationContext<T> context)
    {
        var result = base.Validate(context);
        if (sharedValidators.Count == 0)
        {
            return result;
        }

        var inner = sharedValidators.SelectMany(_ => _.Validate(context.Clone()).Errors);
        MergeErrors(inner, result.Errors);

        return new(result.Errors);
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, Cancel cancel = default)
    {
        var result = await base.ValidateAsync(context, cancel);
        if (sharedValidators.Count == 0)
        {
            return result;
        }

        var inner = new List<ValidationFailure>();
        foreach (var innerValidator in sharedValidators)
        {
            var validationResult = await innerValidator.ValidateAsync(context.Clone(), cancel);
            inner.AddRange(validationResult.Errors);
        }

        MergeErrors(inner, result.Errors);

        return new(result.Errors);
    }

    static void MergeErrors(IEnumerable<ValidationFailure> innerErrors, List<ValidationFailure> errors)
    {
        foreach (var innerError in innerErrors)
        {
            if (errors.Any(_ => _.ErrorMessage == innerError.ErrorMessage))
            {
                continue;
            }

            errors.Add(innerError);
        }
    }
}