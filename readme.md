# <img src="/src/icon.png" height="30px"> ExtendedFluentValidation

[![Build status](https://ci.appveyor.com/api/projects/status/3lr9er83fo8mij5i?svg=true)](https://ci.appveyor.com/project/SimonCropp/ExtendedFluentValidation)
[![NuGet Status](https://img.shields.io/nuget/v/ExtendedFluentValidation.svg)](https://www.nuget.org/packages/ExtendedFluentValidation/)

Extends [FluentValidation](https://fluentvalidation.net/) with some more opinionated rules. 


## Nuget

https://nuget.org/packages/ExtendedFluentValidation/


## Extra Rules

Note: These are a first iteration. Toggling these defaults will be available in a future version.


### Nullability

It leverages nullability information to make all non-nullable reference properties to be required.


### Dates

`DateTime` and `DateTimeOffset` cannot be `MinValue`.


### Strings

String cannot be `String.Empty` or only white-space.


### Guids

Guids cannot be `Guids.Empty`.


## Usage

There are two ways of applying the extended rules.


### ExtendedValidator

Using a base class `ExtendedValidator`:

<!-- snippet: ExtendedValidatorUsage -->
<a id='snippet-extendedvalidatorusage'></a>
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
<sup><a href='/src/Tests/Tests.cs#L281-L292' title='Snippet source file'>snippet source</a> | <a href='#snippet-extendedvalidatorusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### AddExtendedRules

Using an extension method `AddExtendedRules`:

<!-- snippet: AddExtendedRulesUsage -->
<a id='snippet-addextendedrulesusage'></a>
```cs
class PersonValidatorNonBase :
    AbstractValidator<Person>
{
    public PersonValidatorNonBase()
    {
        this.AddExtendedRules();
        //TODO: add any extra rules
    }
}
```
<sup><a href='/src/Tests/Tests.cs#L294-L306' title='Snippet source file'>snippet source</a> | <a href='#snippet-addextendedrulesusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Equivalent

The above are equivalent to:

<!-- snippet: Person -->
<a id='snippet-person'></a>
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
<sup><a href='/src/Tests/Tests.cs#L268-L279' title='Snippet source file'>snippet source</a> | <a href='#snippet-person' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: Equivalent -->
<a id='snippet-equivalent'></a>
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
<sup><a href='/src/Tests/Tests.cs#L308-L328' title='Snippet source file'>snippet source</a> | <a href='#snippet-equivalent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Shared Rules

Given the following models:

<!-- snippet: SharedRulesModels -->
<a id='snippet-sharedrulesmodels'></a>
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
<sup><a href='/src/Tests/SharedRuleTests.cs#L37-L53' title='Snippet source file'>snippet source</a> | <a href='#snippet-sharedrulesmodels' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

It is desirable to have the rules for `IDbRecord` defined separately, and not need to duplicate them for every implementing class. This can be done using Share Rules.

Configure any shared rules at startup:

<!-- snippet: SharedRulesInit -->
<a id='snippet-sharedrulesinit'></a>
```cs
[ModuleInitializer]
public static void Init()
{
    ValidatorExtensions.SharedValidatorFor<IDbRecord>()
        .RuleFor(record => record.RowVersion)
        .Must(rowVersion => rowVersion?.Length == 8)
        .WithMessage("RowVersion must be 8 bytes");
}
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L11-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-sharedrulesinit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The `PersonValidator` used only the standard rules, so needs no constructor.

<!-- snippet: SharedRulesUsage -->
<a id='snippet-sharedrulesusage'></a>
```cs
class PersonValidator :
    ExtendedValidator<Person>
{
}
```
<sup><a href='/src/Tests/SharedRuleTests.cs#L55-L62' title='Snippet source file'>snippet source</a> | <a href='#snippet-sharedrulesusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The above is equivalent to:

<!-- snippet: SharedRulesEquivalent -->
<a id='snippet-sharedrulesequivalent'></a>
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
<sup><a href='/src/Tests/SharedRuleTests.cs#L64-L82' title='Snippet source file'>snippet source</a> | <a href='#snippet-sharedrulesequivalent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Pointed Star](https://thenounproject.com/term/pointed+star/802333/) designed by [Eliricon](https://thenounproject.com/mordarius/) from [The Noun Project](https://thenounproject.com).
