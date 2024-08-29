namespace ExtendedFluentValidation;

public class NotEmptyCollectionValidator<T> :
    PropertyValidator<T, IEnumerable?>,
    INotEmptyValidator
{
    public override string Name => "NotEmptyCollectionValidator";

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

        var enumerator = value.GetEnumerator();
        using var disposable = enumerator as IDisposable;
        return enumerator.MoveNext();
    }

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "'{PropertyName}' must not be empty.";
}