using System.Reflection;

namespace FluentValidation
{
    public abstract class ExtendedValidator<T> :
        AbstractValidator<T>
    {
        protected ExtendedValidator()
        {
            this.AddExtendedRules();
        }

        public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(PropertyInfo property)
        {
            return this.RuleFor<T, TProperty>(property);
        }
    }
}