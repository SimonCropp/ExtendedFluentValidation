namespace ExtendedFluentValidation;

public class NotEmptyCollectionValidator<T> :
    PropertyValidator<T, IEnumerable?>,
    INotEmptyValidator
{
    public override string Name => "NotEmptyValidator";

    public override bool IsValid(ValidationContext<T> context, IEnumerable? value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is ICollection collection)
        {
            return collection.Count > 0;
        }

        return value.GetEnumerator().MoveNext();
    }

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not be empty.";
}