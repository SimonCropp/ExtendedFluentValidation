namespace FluentValidation;

public static class ValidatorExtensions
{
    public static void AddExtendedRules<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T>(
        this AbstractValidator<T> validator,
        IReadOnlyList<string>? exclusions = null,
        bool validateEmptyLists = false) =>
        new RuleBuilder().AddExtendedRules(validator, exclusions,validateEmptyLists);

    public static ValidationContext<T> Clone<T>(this ValidationContext<T> context)
    {
        var innerContext = new ValidationContext<T>(context.InstanceToValidate);
        foreach (var contextItem in context.RootContextData)
        {
            innerContext.RootContextData.Add(contextItem);
        }

        return innerContext;
    }
    public static IRuleBuilderInitial<TTarget, TProperty> RuleFor<TTarget, TProperty>(AbstractValidator<TTarget> validator, PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(TTarget));
        var body = Expression.Property(param, property);
        var expression = Expression.Lambda<Func<TTarget, TProperty>>(body, param);
        return validator.RuleFor(expression);
    }
}