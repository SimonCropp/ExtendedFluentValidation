using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Fact]
    public Task Nulls_NoValues()
    {
        var validator = new TargetWithNullsValidator();

        var result = validator.Validate(new TargetWithNulls());
        return Verifier.Verify(result);
    }

    [Fact]
    public Task Nulls_WithValues()
    {
        var validator = new TargetWithNullsValidator();

        var target = new TargetWithNulls
        {
            ReadWrite = "a", Write = "a"
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    class TargetWithNullsValidator :
        ExtendedValidator<TargetWithNulls>
    {
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
        var validator = new TargetWithNoNullsValidator();

        var result = validator.Validate(new TargetWithNoNulls());
        return Verifier.Verify(result);
    }

    [Fact]
    public Task NoNulls_WithValues()
    {
        var validator = new TargetWithNoNullsValidator();

        var target = new TargetWithNoNulls
        {
            ReadWrite = "a",
            Write = "a"
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    class TargetWithNoNullsValidator :
        ExtendedValidator<TargetWithNoNulls>
    {
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
        var validator = new TargetValueTypesValidator();

        var result = validator.Validate(new TargetValueTypes());
        return Verifier.Verify(result);
    }

    [Fact]
    public Task Guids_NonEmpty()
    {
        var validator = new TargetWithGuidsValidator();

        var target = new TargetWithGuids
        {
            NotNullable = new("25896344-193c-48b3-ab71-53859d347647"),
            Nullable = new Guid("25896344-193c-48b3-ab71-53859d347647")
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result)
            .ModifySerialization(settings => settings.DontScrubGuids());
    }

    [Fact]
    public Task Guids_Defaults()
    {
        var validator = new TargetWithGuidsValidator();

        var target = new TargetWithGuids();
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    [Fact]
    public Task Guids_Empty()
    {
        var validator = new TargetWithGuidsValidator();

        var target = new TargetWithGuids
        {
            NotNullable = Guid.Empty,
            Nullable = Guid.Empty
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    class TargetWithGuidsValidator :
        ExtendedValidator<TargetWithGuids>
    {
    }

    class TargetWithGuids
    {
        public Guid? Nullable { get; set; }
        public Guid NotNullable { get; set; }
    }

    [Fact]
    public Task Strings_NonEmpty()
    {
        var validator = new TargetWithStringsValidator();

        var target = new TargetWithStrings
        {
            NotNullable = "a",
            Nullable = "a"
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    [Fact]
    public Task Strings_Defaults()
    {
        var validator = new TargetWithStringsValidator();

        var target = new TargetWithStrings();
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    [Fact]
    public Task Strings_Empty()
    {
        var validator = new TargetWithStringsValidator();

        var target = new TargetWithStrings
        {
            NotNullable = "",
            Nullable = ""
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    class TargetWithStringsValidator :
        ExtendedValidator<TargetWithStrings>
    {
    }

    class TargetWithStrings
    {
        public string? Nullable { get; set; }
        public string NotNullable { get; set; }
    }

    [Fact]
    public Task ValueTypes_WithValues()
    {
        var validator = new TargetValueTypesValidator();

        var target = new TargetValueTypes
        {
            NotNullable = true,
            Nullable = true,
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }

    class TargetValueTypesValidator :
        ExtendedValidator<TargetValueTypes>
    {
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
        return Verifier.Verify(result)
            .AddScrubber(builder => builder.Replace("1/01/0001", "1/1/0001"));
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
        public PersonValidatorNonBase()
        {
            this.AddExtendedRules();
            //TODO: add any extra rules
        }
    }

    #endregion

    #region Equivalent

    class PersonValidatorEquivalent :
        AbstractValidator<Person>
    {
        public PersonValidatorEquivalent()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);
            RuleFor(x => x.FirstName)
                .NotEmpty();
            RuleFor(x => x.MiddleName)
                .SetValidator(new NotWhiteSpaceValidator<Person>());
            RuleFor(x => x.FamilyName)
                .NotEmpty();
            RuleFor(x => x.Dob)
                .NotEqual(DateTimeOffset.MinValue);
        }
    }

    #endregion
}