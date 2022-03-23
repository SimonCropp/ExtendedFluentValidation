namespace ExtendedFluentValidation;

public class NotNullValidator<T, TProperty> :
    FluentValidation.Validators.NotNullValidator<T, TProperty>
{
    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not be null.";
}