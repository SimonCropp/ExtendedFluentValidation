namespace ExtendedFluentValidation;

public class NotWhiteSpaceStringCollectionValidator<T> :
    PropertyValidator<T, IEnumerable<string?>?>
{
    public override string Name => "NotWhiteSpaceStringCollectionValidator";

    public override bool IsValid(ValidationContext<T> context, IEnumerable<string?>? value)
    {
        if (value == null)
        {
            return true;
        }

        var failure = false;
        var index = -1;

        foreach (var item in value)
        {
            index++;
            if (item is null)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            context.AddFailure($"White space item at index {index}");
            failure = true;
        }

        return failure;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not be empty.";
}