public class StringCollectionAllowEmptyTests
{
    [Fact]
    public Task NonEmpty()
    {
        var validator = new ExtendedValidator<Target>();

        var target = new Target
        {
            NotNullable = ["a"],
            Nullable = ["a"],
            NotNullableAndNullableItem = ["a"],
            NullableAndNullableItem = ["a"]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Defaults()
    {
        var validator = new ExtendedValidator<Target>();

        var target = new Target();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Empty()
    {
        var validator = new ExtendedValidator<Target>();

        var target = new Target
        {
            NotNullable = [],
            Nullable = [],
            NotNullableAndNullableItem = [],
            NullableAndNullableItem = []
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Whitespace()
    {
        var validator = new ExtendedValidator<Target>();

        var target = new Target
        {
            NotNullable = [" "],
            Nullable = [" "],
            NotNullableAndNullableItem = [" "],
            NullableAndNullableItem = [" "]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }
    [Fact]
    public Task NullItem()
    {
        var validator = new ExtendedValidator<Target>();

        var target = new Target
        {
            NotNullable = [null!],
            Nullable = [null!],
            NotNullableAndNullableItem = [null],
            NullableAndNullableItem = [null]
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class Target
    {
        [AllowEmpty]
        public List<string>? Nullable { get; set; }
        [AllowEmpty]
        public List<string?>? NullableAndNullableItem { get; set; }
        [AllowEmpty]
        public List<string> NotNullable { get; set; }
        [AllowEmpty]
        public List<string?> NotNullableAndNullableItem { get; set; }
    }
}