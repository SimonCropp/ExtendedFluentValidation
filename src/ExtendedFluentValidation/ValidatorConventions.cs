namespace FluentValidation;

public static class ValidatorConventions
{
    static Dictionary<Type, IValidator> conventions = [];

    public static AbstractValidator<T> ValidatorFor<T>()
    {
        var type = typeof(T);
        ConstructableValidator<T> extendedValidator;

        if (conventions.TryGetValue(type, out var validator))
        {
            extendedValidator = (ConstructableValidator<T>)validator;
        }
        else
        {
            conventions[type] = extendedValidator = new();
        }

        return extendedValidator;
    }

    public static IEnumerable<IValidator> GetValidatorsFor<TTarget>()
    {
        var type = typeof(TTarget);

        return conventions
            .Where(convention => convention.Key.IsAssignableFrom(type))
            .Select(_ => _.Value);
    }
}