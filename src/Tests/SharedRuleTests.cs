using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentValidation;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SharedRuleTests
{
    #region SharedRules
    [ModuleInitializer]
    public static void Init()
    {
        ValidatorExtensions.SharedValidatorFor<IDbRecord>()
            .RuleFor(record => record.RowVersion)
            .NotEmpty();
    }

    [Fact]
    public Task Usage()
    {
        var validator = new PersonValidator();

        var target = new Person
        {
            Name = "Joe"
        };
        var result = validator.Validate(target);
        return Verifier.Verify(result);
    }
    #endregion

    #region PersonSharedRules
    
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

    class PersonValidator:
        ExtendedValidator<Person>
    {
    }

    #endregion
}
