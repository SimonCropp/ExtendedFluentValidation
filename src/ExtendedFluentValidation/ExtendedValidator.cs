namespace FluentValidation
{
    public abstract class ExtendedValidator<T> :
        AbstractValidator<T>
    {
        protected ExtendedValidator()
        {
            this.AddExtendedRules();
        }
    }
}
