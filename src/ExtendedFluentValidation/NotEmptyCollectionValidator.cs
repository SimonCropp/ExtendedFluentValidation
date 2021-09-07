using System.Collections;
using System.Linq;

namespace FluentValidation.Validators
{
    public class NotEmptyCollectionValidator<T> : PropertyValidator<T, IEnumerable?>, INotEmptyValidator
    {
        public override string Name => "NotEmptyValidator";

        public override bool IsValid(ValidationContext<T> context, IEnumerable? value)
        {
            if (value == null)
            {
                return true;
            }

            return value.Cast<object>().Any();
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return Localized(errorCode, Name);
        }
    }
}