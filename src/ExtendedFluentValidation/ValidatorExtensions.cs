﻿namespace FluentValidation;

public static class ValidatorExtensions
{
    public static void AddExtendedRules<[DynamicMembers(DynamicTypes.PublicProperties | DynamicTypes.NonPublicProperties)] T>(
        this AbstractValidator<T> validator,
        IReadOnlyList<string>? exclusions = null,
        bool validateEmptyLists = false)
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
        AddNotDefaultDate<T, DateTime>(validator, otherProperties);
        AddNotDefaultDate<T, DateTimeOffset>(validator, otherProperties);
#if(NET6_0_OR_GREATER)
        AddNotDefaultDate<T, Date>(validator, otherProperties);
#endif
    }

    static void NotNull<T>(AbstractValidator<T> validator, bool validateEmptyLists, List<PropertyInfo> notNullProperties)
    {
        foreach (var property in notNullProperties)
        {
            if (property.IsString())
            {
                var ruleFor = validator.RuleFor<T, string>(property);
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
                validator.RuleFor(property).NotNull();
                var ruleFor = validator.RuleFor<T, IEnumerable>(property);
                ruleFor.SetValidator(new NotEmptyCollectionValidator<T>());
            }
            else
            {
                validator.RuleFor(property).NotNull();
            }
        }
    }

    static void NotWhiteSpace<T>(AbstractValidator<T> validator, List<PropertyInfo> otherProperties)
    {
        var stringProperties = otherProperties
            .Where(_ => _.IsString());
        foreach (var property in stringProperties)
        {
            if (!property.AllowsEmpty())
            {
                var ruleFor = validator.RuleFor<T, string?>(property);
                ruleFor.SetValidator(new NotWhiteSpaceValidator<T>());
            }
        }
    }

    static void NotEmptyCollections<T>(AbstractValidator<T> validator, List<PropertyInfo> otherProperties)
    {
        var collectionProperties = otherProperties
            .Where(_ => !_.IsString() &&
                        _.IsCollection());
        foreach (var property in collectionProperties)
        {
            var ruleFor = validator.RuleFor<T, IEnumerable>(property);
            ruleFor.SetValidator(new NotEmptyCollectionValidator<T>());
        }
    }

    public static ValidationContext<T> Clone<T>(this ValidationContext<T> context)
    {
        var innerContext = new ValidationContext<T>(context.InstanceToValidate);
        foreach (var contextItem in context.RootContextData)
        {
            innerContext.RootContextData.Add(contextItem);
        }

        return innerContext;
    }

    static void AddNotEmptyGuid<TTarget>(AbstractValidator<TTarget> validator, List<PropertyInfo> properties)
    {
        properties = properties
            .Where(_ => !_.AllowsEmpty())
            .ToList();

        var typedProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid));
        foreach (var property in typedProperties)
        {
            validator.RuleFor<TTarget, Guid>(property)
                .NotEqual(default(Guid))
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(Guid?));
        foreach (var property in typedNullableProperties)
        {
            validator.RuleFor<TTarget, Guid?>(property)
                .NotEqual(default(Guid))
                .WithMessage($"{property.Name} must not be `Guid.Empty`.");
        }
    }

    static void AddNotDefaultDate<TTarget, TProperty>(AbstractValidator<TTarget> validator, List<PropertyInfo> properties)
        where TProperty : struct
    {
        var type = typeof(TProperty);
        var typedProperties = properties
            .Where(_ => _.PropertyType == type);
        foreach (var property in typedProperties)
        {
            validator.RuleFor<TTarget, TProperty>(property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }

        var typedNullableProperties = properties
            .Where(_ => _.PropertyType == typeof(TProperty?));
        foreach (var property in typedNullableProperties)
        {
            validator.RuleFor<TTarget, TProperty?>(property)
                .NotEqual(default(TProperty))
                .WithMessage($"{property.Name} must not be `{type.Name}.MinValue`.");
        }
    }

    static IRuleBuilderInitial<TTarget, object> RuleFor<TTarget>(this AbstractValidator<TTarget> validator, PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(TTarget));
        var body = Expression.Property(param, property);
        var converted = Expression.Convert(body, typeof(object));
        var expression = Expression.Lambda<Func<TTarget, object>>(converted, param);
        return validator.RuleFor(expression);
    }

    public static IRuleBuilderInitial<TTarget, TProperty> RuleFor<TTarget, TProperty>(this AbstractValidator<TTarget> validator, PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(TTarget));
        var body = Expression.Property(param, property);
        var expression = Expression.Lambda<Func<TTarget, TProperty>>(body, param);
        return validator.RuleFor(expression);
    }
}