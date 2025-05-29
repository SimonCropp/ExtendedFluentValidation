[TestFixture]
public partial class Tests
{
    [Test]
    public Task Array_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithArrays>();

        var target = new TargetWithArrays
        {
            NotNullable = ["a"],
            Nullable = ["a"]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task Array_NonEmpty_ValidateEmptyArrays()
    {
        var validator = new ExtendedValidator<TargetWithArrays>(validateEmptyLists: true);

        var target = new TargetWithArrays
        {
            NotNullable = ["a"],
            Nullable = ["a"]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task Array_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithArrays>();

        var target = new TargetWithArrays();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task Array_Defaults_ValidateEmptyArrays()
    {
        var validator = new ExtendedValidator<TargetWithArrays>(validateEmptyLists: true);

        var target = new TargetWithArrays();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task Array_Empty_ValidateEmptyArrays()
    {
        var validator = new ExtendedValidator<TargetWithArrays>(validateEmptyLists: true);

        var target = new TargetWithArrays
        {
            NotNullable = [],
            Nullable = []
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task Array_Empty()
    {
        var validator = new ExtendedValidator<TargetWithArrays>();

        var target = new TargetWithArrays
        {
            NotNullable = [],
            Nullable = []
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithArrays
    {
        public string[]? Nullable { get; init; }
        public string[] NotNullable { get; init; }
    }
}