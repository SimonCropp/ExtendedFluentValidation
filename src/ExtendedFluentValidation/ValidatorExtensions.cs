using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Validators;

namespace FluentValidation
{
    public static class ValidatorExtensions
    {
        static Dictionary<Type, IValidator> conventions = new();

        public static AbstractValidator<T> SharedValidatorFor<T>()
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

        public static IEnumerable<IValidator> GetSharedValidatorsFor<TTarget>()
        {
            var type = typeof(TTarget);

            return conventions
                .Where(convention => convention.Key.IsAssignableFrom(type))
                .Select(x => x.Value);
        }

        public static void AddExtendedRules<T>(this AbstractValidator<T> validator)
        {
            var properties = Extensions.GettableProperties<T>();

            var notNullProperties = properties
                .Where(x => x.PropertyType.IsClass &&
                            x.GetNullabilityInfo().ReadState == NullabilityState.NotNull)
                .ToList();
            foreach (var property in notNullProperties)
            {
                if (property.PropertyType == typeof(string))
                {
                    validator.RuleFor<T, string>(property).NotEmpty();
                }
                else
                {
                    validator.RuleFor(property).NotNull();
                }
            }

            var otherProperties = properties.Except(notNullProperties).ToList();

            var stringProperties = otherProperties
                .Where(x => x.PropertyType == typeof(string));
            foreach (var property in stringProperties)
            {
                var ruleFor = validator.RuleFor<T, string?>(property);
                ruleFor.SetValidator(new NotWhiteSpaceValidator<T>());
            }

            AddNotEquals<T, Guid>(validator, otherProperties);
            AddNotEquals<T, DateTime>(validator, otherProperties);
            AddNotEquals<T, DateTimeOffset>(validator, otherProperties);
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

        static void AddNotEquals<TTarget, TProperty>(AbstractValidator<TTarget> validator, List<PropertyInfo> properties)
            where TProperty : struct
        {
            var typedProperties = properties
                .Where(x => x.PropertyType == typeof(TProperty));
            foreach (var property in typedProperties)
            {
                var ruleFor = validator.RuleFor<TTarget, TProperty>(property);
                ruleFor.NotEqual(default(TProperty));
            }

            var typedNullableProperties = properties
                .Where(x => x.PropertyType == typeof(TProperty?));
            foreach (var property in typedNullableProperties)
            {
                var ruleFor = validator.RuleFor<TTarget, TProperty?>(property);
                ruleFor.NotEqual(default(TProperty));
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
}