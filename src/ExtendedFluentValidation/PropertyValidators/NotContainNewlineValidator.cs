namespace FluentValidation;

public class NotContainNewlineValidator<T> :
    PropertyValidator<T, string?>
{
    // ReSharper disable once RedundantExplicitParamsArrayCreation
    static readonly SearchValues<char> newlines = SearchValues.Create(['\r', '\n']);

    public override string Name => "NotContainNewlineValidator";

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (value == null)
        {
            return true;
        }

        return !value.ContainsAny(newlines);
    }

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not contain new line characters.";
}