using FluentValidation;

[UsesVerify]
public class SharedRuleTests
{
    #region SharedRulesInit

    [ModuleInitializer]
    public static void Init()
    {
        ValidatorExtensions.SharedValidatorFor<IDbRecord>()
            .RuleFor(record => record.RowVersion)
            .Must(rowVersion => rowVersion?.Length == 8)
            .WithMessage("RowVersion must be 8 bytes");
    }

    #endregion

    [Fact]
    public Task Usage()
    {
        var validator = new PersonValidator();

        var target = new Person
        {
            Name = "Joe"
        };
        var result = validator.Validate(target);
        return Verify(result);
    }

    #region SharedRulesModels

    public interface IDbRecord
    {
        public byte[] RowVersion { get; }
        public Guid Id { get; }
    }

    public class Person :
        IDbRecord
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] RowVersion { get; set; }
    }

    #endregion

    #region SharedRulesUsage

    class PersonValidator :
        ExtendedValidator<Person>
    {
    }

    #endregion

    #region SharedRulesEquivalent

    class PersonValidatorEquivalent :
        AbstractValidator<Person>
    {
        public PersonValidatorEquivalent()
        {
            RuleFor(_ => _.Id)
                .NotEqual(Guid.Empty);
            RuleFor(_ => _.Name)
                .NotEmpty();
            RuleFor(_ => _.RowVersion)
                .NotNull()
                .Must(rowVersion => rowVersion?.Length == 8)
                .WithMessage("RowVersion must be 8 bytes");
        }
    }

    #endregion
}