namespace FluentValidation;

public class RuleBuilder<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T>
{
    AbstractValidator<T> validator;
    bool validateEmptyLists;

    public RuleBuilder(
        AbstractValidator<T> validator,
        IReadOnlyList<string>? exclusions,
        bool? validateEmptyLists)
    {
        this.validator = validator;
        this.validateEmptyLists = ValidatorConventions.ShouldValidateEmptyLists(validateEmptyLists);
        var properties = Extensions.GettableProperties<T>(exclusions);

        var notNullProperties = properties
            .Where(_ => _.NotNullable())
            .ToList();
        NotNull(notNullProperties);

        var otherProperties = properties.Except(notNullProperties).ToList();

        NotWhiteSpace(otherProperties);
        if (this.validateEmptyLists)
        {
            NotEmptyCollections(otherProperties);
        }

        AddNotEmptyGuid(otherProperties);
        AddNotDefaultDate<DateTime>(otherProperties);
        AddNotDefaultDate<DateTimeOffset>(otherProperties);
#if(NET6_0_OR_GREATER)
        AddNotDefaultDate<Date>(otherProperties);
#endif
    }

    void NotNull(List<PropertyInfo> notNullProperties)
    {
        foreach (var property in notNullProperties)
        {
            if (property.IsString())
            {
                var ruleFor = RuleFor<string>(property);
                if (property.AllowsEmpty())
                {
                    ruleFor.SetValidator(new ExtendedFluentValidation.NotNullValidator<T, string>());
                }
                else
                {
                    ruleFor.NotEmpty();
                }
                continue;
            }

            if (property.IsCollection())
            {
                if (validateEmptyLists)
                {
                    RuleFor<IEnumerable>(property)
                        .NotEmpty();
                }
                else
                {
                    RuleFor<IEnumerable>(property).NotNull();
                }
                continue;
            }

            RuleFor<object>(property).NotNull();
        }
    }

    void NotWhiteSpace(List<PropertyInfo> otherProperties)
    {
        foreach (var property in otherProperties
                     .Where(_ => _.IsString() &&
                                 !_.AllowsEmpty()))
        {
            RuleFor<string?>(property)
                .SetValidator(new NotWhiteSpaceValidator<T>());
        }
    }

    void NotEmptyCollections(List<PropertyInfo> otherProperties)
    {
        var collectionProperties = otherProperties
            .Where(_ => !_.IsString() &&
                        _.IsCollection());
        foreach (var property in collectionProperties)
        {
            RuleFor<IEnumerable>(property)
                .SetValidator(new NotEmptyCollectionValidator<T>());
        }
    }

    void AddNotEmptyGuid(List<PropertyInfo> properties)
    {
        properties = properties
            .Where(_ => !_.AllowsEmpty())
            .ToList();

        var typedProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid));
        foreach (var property in typedProperties)
        {
            RuleFor<Guid>(property)
                .NotEqual(Guid.Empty)
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid?));
        foreach (var property in typedNullableProperties)
        {
            RuleFor<Guid?>(property)
                .NotEqual(Guid.Empty)
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }
    }

    void AddNotDefaultDate<TProperty>(List<PropertyInfo> properties)
        where TProperty : struct
    {
        var type = typeof(TProperty);
        var typedProperties = properties
            .Where(_ => _.PropertyType == type);
        foreach (var property in typedProperties)
        {
            RuleFor<TProperty>(property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(TProperty?));
        foreach (var property in typedNullableProperties)
        {
            RuleFor<TProperty?>(property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }
    }

    static ParameterExpression param = Expression.Parameter(typeof(T));

    internal IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property)
    {
        var body = Expression.Property(param, property);
        var expression = Expression.Lambda<Func<T, TProperty>>(body, param);
        return validator.RuleFor(expression);
    }
}