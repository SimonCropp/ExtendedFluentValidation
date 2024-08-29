// ReSharper disable MemberCanBeMadeStatic.Local
namespace FluentValidation;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class RuleBuilder<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T>
{
    public void AddExtendedRules(
        AbstractValidator<T> validator,
        IReadOnlyList<string>? exclusions,
        bool validateEmptyLists)
    {
        var properties = Extensions.GettableProperties<T>(exclusions);

        var notNullProperties = properties
            .Where(_ => _.NotNullable())
            .ToList();
        NotNull(validator, validateEmptyLists, notNullProperties);

        var otherProperties = properties.Except(notNullProperties).ToList();

        NotWhiteSpace(validator, otherProperties);
        if (validateEmptyLists)
        {
            NotEmptyCollections(validator, otherProperties);
        }
        AddNotEmptyGuid(validator, otherProperties);
        AddNotDefaultDate<DateTime>(validator, otherProperties);
        AddNotDefaultDate<DateTimeOffset>(validator, otherProperties);
#if(NET6_0_OR_GREATER)
        AddNotDefaultDate<Date>(validator, otherProperties);
#endif
    }

    void NotNull(AbstractValidator<T> validator, bool validateEmptyLists, List<PropertyInfo> notNullProperties)
    {
        foreach (var property in notNullProperties)
        {
            if (property.IsString())
            {
                var ruleFor = RuleFor<string>(validator, property);
                if (property.AllowsEmpty())
                {
                    ruleFor.SetValidator(new ExtendedFluentValidation.NotNullValidator<T,string>());
                }
                else
                {
                    ruleFor.NotEmpty();
                }
            }
            else if (validateEmptyLists && property.IsCollection())
            {
                RuleFor(validator, property).NotNull();
                var ruleFor = RuleFor<IEnumerable>(validator, property);
                ruleFor.SetValidator(new NotEmptyCollectionValidator<T>());
            }
            else
            {
                RuleFor(validator, property).NotNull();
            }
        }
    }

    void NotWhiteSpace(AbstractValidator<T> validator, List<PropertyInfo> otherProperties)
    {
        var stringProperties = otherProperties
            .Where(_ => _.IsString());
        foreach (var property in stringProperties)
        {
            if (!property.AllowsEmpty())
            {
                var ruleFor = RuleFor<string?>(validator, property);
                ruleFor.SetValidator(new NotWhiteSpaceValidator<T>());
            }
        }
    }

    void NotEmptyCollections(AbstractValidator<T> validator, List<PropertyInfo> otherProperties)
    {
        var collectionProperties = otherProperties
            .Where(_ => !_.IsString() &&
                        _.IsCollection());
        foreach (var property in collectionProperties)
        {
            var ruleFor = RuleFor<IEnumerable>(validator, property);
            ruleFor.SetValidator(new NotEmptyCollectionValidator<T>());
        }
    }

    void AddNotEmptyGuid(AbstractValidator<T> validator, List<PropertyInfo> properties)
    {
        properties = properties
            .Where(_ => !_.AllowsEmpty())
            .ToList();

        var typedProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid));
        foreach (var property in typedProperties)
        {
            RuleFor<Guid>(validator, property)
                .NotEqual(default(Guid))
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid?));
        foreach (var property in typedNullableProperties)
        {
            RuleFor<Guid?>(validator, property)
                .NotEqual(default(Guid))
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }
    }

    void AddNotDefaultDate<TProperty>(AbstractValidator<T> validator, List<PropertyInfo> properties)
        where TProperty : struct
    {
        var type = typeof(TProperty);
        var typedProperties = properties
            .Where(_ => _.PropertyType == type);
        foreach (var property in typedProperties)
        {
            RuleFor<TProperty>(validator, property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(TProperty?));
        foreach (var property in typedNullableProperties)
        {
            RuleFor<TProperty?>(validator, property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }
    }

    IRuleBuilderInitial<T, object> RuleFor(AbstractValidator<T> validator, PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.Property(param, property);
        var converted = Expression.Convert(body, typeof(object));
        var expression = Expression.Lambda<Func<T, object>>(converted, param);
        return validator.RuleFor(expression);
    }

    IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(AbstractValidator<T> validator, PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.Property(param, property);
        var expression = Expression.Lambda<Func<T, TProperty>>(body, param);
        return validator.RuleFor(expression);
    }
}