# <img src="/src/icon.png" height="30px"> ExtendedFluentValidation

[![Build status](https://ci.appveyor.com/api/projects/status/3lr9er83fo8mij5i?svg=true)](https://ci.appveyor.com/project/SimonCropp/ExtendedFluentValidation)
[![NuGet Status](https://img.shields.io/nuget/v/ExtendedFluentValidation.svg)](https://www.nuget.org/packages/ExtendedFluentValidation/)

Extends [FluentValidation](https://fluentvalidation.net/) with some more opinionated rules and extensions.

**See [Milestones](../../milestones?state=closed) for release notes.**


## Nuget

https://nuget.org/packages/ExtendedFluentValidation/


## Extra Rules


### Nullability

It leverages nullability information to make all non-nullable reference properties to be required.


### Dates

`DateTime`, `DateTimeOffset`, and `DateOnly` cannot be `MinValue`.


### Strings

String cannot be `String.Empty` or only white-space. The logic being: if the absence of text is valid, then make the member nullable. This helps since nullable is a first class strong type feature, where "string is empty or only white-space" is a runtime check.


### Guids

Guids cannot be `Guid.Empty`.


### Lists/Collections

Lists and Collection cannot be empty if `ValidatorConventions.ValidateEmptyLists()` is called in a module initializer. The logic being: if the absence of any values is valid, then make the member nullable. This helps since nullable is a first class strong type feature, where "list contains no values" is a runtime check.


## Usage

There are two ways of applying the extended rules.


### ExtendedValidator

Using a base class `ExtendedValidator`:

<!-- snippet: ExtendedValidatorUsage -->
<a id='snippet-ExtendedValidatorUsage'></a>
```cs
class PersonValidatorFromBase :
    ExtendedValidator<Person>
{
    public PersonValidatorFromBase()
    {
        //TODO: add any extra rules
    }
}
```
<sup><a href='/src/Tests/Tests.cs#L489-L500' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExtendedValidatorUsage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### AddExtendedRules

Using an extension method `AddExtendedRules`:

<!-- snippet: AddExtendedRulesUsage -->
<a id='snippet-AddExtendedRulesUsage'></a>
```cs
class PersonValidatorNonBase :
    AbstractValidator<Person>
{
    public PersonValidatorNonBase() =>
        this.AddExtendedRules();
    //TODO: add any extra rules
}
```
<sup><a href='/src/Tests/Tests.cs#L502-L512' title='Snippet source file'>snippet source</a> | <a href='#snippet-AddExtendedRulesUsage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Equivalent

The above are equivalent to:

<!-- snippet: Person -->
<a id='snippet-Person'></a>
```cs
public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string FamilyName { get; set; }
    public DateTimeOffset Dob { get; set; }
}
```
<sup><a href='/src/Tests/Tests.cs#L474-L485' title='Snippet source file'>snippet source</a> | <a href='#snippet-Person' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: Equivalent -->
<a id='snippet-Equivalent'></a>
```cs
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
```
<sup><a href='/src/Tests/Tests.cs#L514-L534' title='Snippet source file'>snippet source</a> | <a href='#snippet-Equivalent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Shared Rules

Given the following models:

<!-- snippet: SharedRulesModels -->
<a id='snippet-SharedRulesModels'></a>
```cs
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
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L28-L44' title='Snippet source file'>snippet source</a> | <a href='#snippet-SharedRulesModels' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

It is desirable to have the rules for `IDbRecord` defined separately, and not need to duplicate them for every implementing class. This can be done using shares rules.

Configure any shared rules at startup:

<!-- snippet: SharedRulesInit -->
<a id='snippet-SharedRulesInit'></a>
```cs
[ModuleInitializer]
public static void Init() =>
    ValidatorConventions.ValidatorFor<IDbRecord>()
        .RuleFor(record => record.RowVersion)
        .Must(rowVersion => rowVersion?.Length == 8)
        .WithMessage("RowVersion must be 8 bytes");
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L4-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-SharedRulesInit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The `PersonValidator` used only the standard rules, so needs no constructor.

<!-- snippet: SharedRulesUsage -->
<a id='snippet-SharedRulesUsage'></a>
```cs
class PersonValidator :
    ExtendedValidator<Person>;
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L46-L51' title='Snippet source file'>snippet source</a> | <a href='#snippet-SharedRulesUsage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The above is equivalent to:

<!-- snippet: SharedRulesEquivalent -->
<a id='snippet-SharedRulesEquivalent'></a>
```cs
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
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L53-L71' title='Snippet source file'>snippet source</a> | <a href='#snippet-SharedRulesEquivalent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Pointed Star](https://thenounproject.com/term/pointed+star/802333/) designed by [Eliricon](https://thenounproject.com/mordarius/) from [The Noun Project](https://thenounproject.com).
