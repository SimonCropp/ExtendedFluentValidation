[TestFixture]
public partial class Tests
{
    [Test]
    public Task List_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithLists>();

        var target = new TargetWithLists
        {
            NotNullable = ["a"],
            Nullable = ["a"]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task List_NonEmpty_ValidateEmptyLists()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists
        {
            NotNullable = ["a"],
            Nullable = ["a"]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task List_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithLists>();

        var target = new TargetWithLists();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task List_Defaults_ValidateEmptyLists()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task List_Empty_ValidateEmptyLists()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists
        {
            NotNullable = [],
            Nullable = []
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Test]
    public Task List_Empty()
    {
        var validator = new ExtendedValidator<TargetWithLists>();

        var target = new TargetWithLists
        {
            NotNullable = [],
            Nullable = []
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithLists
    {
        public List<string>? Nullable { get; init; }
        public List<string> NotNullable { get; init; }
    }
}