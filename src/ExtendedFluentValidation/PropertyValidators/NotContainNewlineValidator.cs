namespace FluentValidation;

public class NotContainNewlineValidator<T> :
    PropertyValidator<T, string?>
{
    public override string Name => "NotContainNewlineValidator";

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (value == null)
        {
            return true;
        }

        return !value.Contains('\n') &&
               !value.Contains('\r');
    }

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not contain new line characters.";
}