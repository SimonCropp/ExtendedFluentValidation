[UsesVerify]
public class Tests
{
    [Fact]
    public Task Nulls_NoValues()
    {
        var validator = new ExtendedValidator<TargetWithNulls>();

        var result = validator.Validate(new TargetWithNulls());
        return Verify(result);
    }

    [Fact]
    public Task Nulls_WithValues()
    {
        var validator = new ExtendedValidator<TargetWithNulls>();

        var target = new TargetWithNulls
        {
            ReadWrite = "a",
            Write = "a"
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithNulls
    {
        string? write;
        public string? ReadWrite { get; set; }
        public string? Read { get; }

        public string? Write
        {
            set => write = value;
        }
    }

    [Fact]
    public Task NoNulls_NoValues()
    {
        var validator = new ExtendedValidator<TargetWithNoNulls>();

        var result = validator.Validate(new TargetWithNoNulls());
        return Verify(result);
    }

    [Fact]
    public Task NoNulls_WithValues()
    {
        var validator = new ExtendedValidator<TargetWithNoNulls>();

        var target = new TargetWithNoNulls
        {
            ReadWrite = "a",
            Write = "a"
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithNoNulls
    {
        string write;
        public string ReadWrite { get; set; }
        public string Read { get; }

        public string Write
        {
            set => write = value;
        }
    }

    [Fact]
    public Task ValueTypes_NoValues()
    {
        var validator = new ExtendedValidator<TargetValueTypes>();

        var result = validator.Validate(new TargetValueTypes());
        return Verify(result);
    }

    [Fact]
    public Task Disabled_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithDisabled>();

        var target = new TargetWithDisabled
        {
            NotNullable = new("25896344-193c-48b3-ab71-53859d347647"),
            Nullable = new Guid("25896344-193c-48b3-ab71-53859d347647")
        };
        var result = validator.Validate(target);
        return Verify(result)
            .DontScrubGuids();
    }

    [Fact]
    public Task Disabled_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithDisabled>();

        var target = new TargetWithDisabled();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Disabled_Empty()
    {
        var validator = new ExtendedValidator<TargetWithDisabled>();

        var target = new TargetWithDisabled
        {
            NotNullable = Guid.Empty,
            Nullable = Guid.Empty
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

#nullable disable
    class TargetWithDisabled
    {
        public Guid? Nullable { get; set; }
        public Guid NotNullable { get; set; }
    }
#nullable enable
    [Fact]
    public Task Dates_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates
        {
            NotNullableDateTime = new(2000, 1, 1),
            NotNullableDateTimeOffset = new DateTime(2000, 1, 1),
            NotNullableDateOnly = new(2000, 1, 1),
            NullableDateTime = new DateTime(2000, 1, 1),
            NullableDateTimeOffset = new DateTime(2000, 1, 1),
            NullableDateOnly = new DateOnly(2000, 1, 1),
            NotNullableAllowEmptyDateTime = new(2000, 1, 1),
            NotNullableAllowEmptyDateTimeOffset = new DateTime(2000, 1, 1),
            NotNullableAllowEmptyDateOnly = new(2000, 1, 1),
            NullableAllowEmptyDateTime = new DateTime(2000, 1, 1),
            NullableAllowEmptyDateTimeOffset = new DateTime(2000, 1, 1),
            NullableAllowEmptyDateOnly = new DateOnly(2000, 1, 1)
        };
        var result = validator.Validate(target);
        return Verify(result)
            .DontScrubGuids();
    }

    [Fact]
    public Task Dates_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Dates_Min()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates
        {
            NotNullableDateTime = DateTime.MinValue,
            NotNullableDateTimeOffset = DateTimeOffset.MinValue,
            NotNullableDateOnly = DateOnly.MinValue,
            NullableDateTime = DateTime.MinValue,
            NullableDateTimeOffset = DateTimeOffset.MinValue,
            NullableDateOnly = DateOnly.MinValue,
            NotNullableAllowEmptyDateTime = DateTime.MinValue,
            NotNullableAllowEmptyDateTimeOffset = DateTimeOffset.MinValue,
            NotNullableAllowEmptyDateOnly = DateOnly.MinValue,
            NullableAllowEmptyDateTime = DateTime.MinValue,
            NullableAllowEmptyDateTimeOffset = DateTimeOffset.MinValue,
            NullableAllowEmptyDateOnly = DateOnly.MinValue
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithDates
    {
        public DateTime? NullableDateTime { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public DateOnly? NullableDateOnly { get; set; }
        public DateTime NotNullableDateTime { get; set; }
        public DateTimeOffset NotNullableDateTimeOffset { get; set; }
        public DateOnly NotNullableDateOnly { get; set; }

        [AllowEmpty]
        public DateTime? NullableAllowEmptyDateTime { get; set; }

        [AllowEmpty]
        public DateTimeOffset? NullableAllowEmptyDateTimeOffset { get; set; }

        [AllowEmpty]
        public DateOnly? NullableAllowEmptyDateOnly { get; set; }

        [AllowEmpty]
        public DateTime NotNullableAllowEmptyDateTime { get; set; }

        [AllowEmpty]
        public DateTimeOffset NotNullableAllowEmptyDateTimeOffset { get; set; }

        [AllowEmpty]
        public DateOnly NotNullableAllowEmptyDateOnly { get; set; }
    }

    [Fact]
    public Task Guids_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithGuids>();

        var target = new TargetWithGuids
        {
            NotNullable = new("25896344-193c-48b3-ab71-53859d347647"),
            Nullable = new Guid("25896344-193c-48b3-ab71-53859d347647"),
            NotNullableAllowEmpty = new("25896344-193c-48b3-ab71-53859d347647"),
            NullableAllowEmpty = new Guid("25896344-193c-48b3-ab71-53859d347647")
        };
        var result = validator.Validate(target);
        return Verify(result)
            .DontScrubGuids();
    }

    [Fact]
    public Task Guids_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithGuids>();

        var target = new TargetWithGuids();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Guids_Empty()
    {
        var validator = new ExtendedValidator<TargetWithGuids>();

        var target = new TargetWithGuids
        {
            NotNullable = Guid.Empty,
            Nullable = Guid.Empty,
            NotNullableAllowEmpty = Guid.Empty,
            NullableAllowEmpty = Guid.Empty
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithGuids
    {
        public Guid? Nullable { get; set; }
        public Guid NotNullable { get; set; }

        [AllowEmpty]
        public Guid? NullableAllowEmpty { get; set; }

        [AllowEmpty]
        public Guid NotNullableAllowEmpty { get; set; }
    }

    [Fact]
    public Task List_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists
        {
            NotNullable = new()
            {
                "a"
            },
            Nullable = new()
            {
                "a"
            }
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task List_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task List_Empty()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists
        {
            NotNullable = new(),
            Nullable = new()
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithLists
    {
        public List<string>? Nullable { get; set; }
        public List<string> NotNullable { get; set; }
    }

    [Fact]
    public Task Strings_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithStrings>();

        var target = new TargetWithStrings
        {
            NotNullable = "a",
            Nullable = "a",
            NotNullableAllowEmpty = "a",
            NullableAllowEmpty = "a"
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Strings_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithStrings>();

        var target = new TargetWithStrings();
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Strings_Empty()
    {
        var validator = new ExtendedValidator<TargetWithStrings>();

        var target = new TargetWithStrings
        {
            NullableAllowEmpty = "",
            NotNullableAllowEmpty = "",
            NotNullable = "",
            Nullable = "",
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    [Fact]
    public Task Strings_Whitespace()
    {
        var validator = new ExtendedValidator<TargetWithStrings>();

        var target = new TargetWithStrings
        {
            NullableAllowEmpty = " ",
            NotNullableAllowEmpty = " ",
            NotNullable = " ",
            Nullable = " ",
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetWithStrings
    {
        public string? Nullable { get; set; }
        public string NotNullable { get; set; }

        [AllowEmpty]
        public string? NullableAllowEmpty { get; set; }

        [AllowEmpty]
        public string NotNullableAllowEmpty { get; set; }
    }

    [Fact]
    public Task Compounded()
    {
        var validator = new TargetCompoundedValidator();

        var target = new TargetCompounded
        {
            Property1 = null!,
            Property2 = "123"
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetCompoundedValidator :
        ExtendedValidator<TargetCompounded>
    {
        public TargetCompoundedValidator()
        {
            RuleFor(_ => _.Property1).MaximumLength(2);
            RuleFor(_ => _.Property2).MaximumLength(2);
        }
    }

    class TargetCompounded
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    [Fact]
    public Task ValueTypes_WithValues()
    {
        var validator = new ExtendedValidator<TargetValueTypes>();

        var target = new TargetValueTypes
        {
            NotNullable = true,
            Nullable = true,
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    class TargetValueTypes
    {
        public bool? Nullable { get; set; }
        public bool NotNullable { get; set; }
    }

    [Fact]
    public Task Usage()
    {
        var validator = new PersonValidatorFromBase();

        var target = new Person
        {
            FirstName = "Joe"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .AddScrubber(builder => builder.Replace("1/1/0001", "1/01/0001"));
    }

    #region Person

    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string FamilyName { get; set; }
        public DateTimeOffset Dob { get; set; }
    }

    #endregion

    #region ExtendedValidatorUsage

    class PersonValidatorFromBase :
        ExtendedValidator<Person>
    {
        public PersonValidatorFromBase()
        {
            //TODO: add any extra rules
        }
    }

    #endregion

    #region AddExtendedRulesUsage

    class PersonValidatorNonBase :
        AbstractValidator<Person>
    {
        public PersonValidatorNonBase() =>
            this.AddExtendedRules();
        //TODO: add any extra rules
    }

    #endregion

    #region Equivalent

    class PersonValidatorEquivalent :
        AbstractValidator<Person>
    {
        public PersonValidatorEquivalent()
        {
            RuleFor(_ => _.Id)
                .NotEqual(Guid.Empty);
            RuleFor(_ => _.FirstName)
                .NotEmpty();
            RuleFor(_ => _.MiddleName)
                .SetValidator(new NotWhiteSpaceValidator<Person>());
            RuleFor(_ => _.FamilyName)
                .NotEmpty();
            RuleFor(_ => _.Dob)
                .NotEqual(DateTimeOffset.MinValue);
        }
    }

    #endregion
}