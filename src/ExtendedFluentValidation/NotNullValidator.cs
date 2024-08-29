namespace ExtendedFluentValidation;

public class NotNullValidator<T, TProperty> :
    FluentValidation.Validators.NotNullValidator<T, TProperty>
{
    public override string Name => "NotNullValidator";

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not be null.";
}