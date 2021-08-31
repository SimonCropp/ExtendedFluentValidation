using System.Reflection;

namespace FluentValidation
{
    public class ExtendedValidator<T> :
        AbstractValidator<T>
    {
        public ExtendedValidator()
        {
            this.AddExtendedRules();
        }

        public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property)
        {
            return this.RuleFor<T, TProperty>(property);
        }
    }
}