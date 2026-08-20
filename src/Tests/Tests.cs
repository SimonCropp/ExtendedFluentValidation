[TestFixture]
public class Tests
{
    [Test]
    public Task Nulls_NoValues()
    {
        var validator = new ExtendedValidator<TargetWithNulls>();

        var result = validator.Validate(new TargetWithNulls());
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Nulls_WithValues()
    {
        var validator = new ExtendedValidator<TargetWithNulls>();

        var target = new TargetWithNulls
        {
            ReadWrite = "a",
            Write = "a"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    class TargetWithNulls
    {
        // ReSharper disable once NotAccessedField.Local
        string? write;
        public string? ReadWrite { get; set; }
        public string? Read { get; }

        public string? Write
        {
            set => write = value;
        }
    }

    [Test]
    public Task WithRecord()
    {
        var validator = new ExtendedValidator<TargetRecord>();

        var target = new TargetRecord("Value");
        var result = validator.Validate(target);
        var rules = validator.ToList();
        ClassicAssert.AreEqual(1, rules.Count);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    record TargetRecord(string Member);

    [Test]
    public Task NoNulls_NoValues()
    {
        var validator = new ExtendedValidator<TargetWithNoNulls>();

        var result = validator.Validate(new TargetWithNoNulls());
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: ReadWrite,
                      ErrorMessage: 'Read Write' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Read Write,
                        PropertyPath: ReadWrite,
                        PropertyValue: null
                      }
                    },
                    {
                      PropertyName: Read,
                      ErrorMessage: 'Read' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Read,
                        PropertyPath: Read,
                        PropertyValue: null
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task NoNulls_WithValues()
    {
        var validator = new ExtendedValidator<TargetWithNoNulls>();

        var target = new TargetWithNoNulls
        {
            ReadWrite = "a",
            Write = "a"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: Read,
                      ErrorMessage: 'Read' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Read,
                        PropertyPath: Read,
                        PropertyValue: null
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    class TargetWithNoNulls
    {
        // ReSharper disable once NotAccessedField.Local
        string write;
        public string ReadWrite { get; set; }
        public string Read { get; }

        public string Write
        {
            set => write = value;
        }
    }

    [Test]
    public Task ValueTypes_NoValues()
    {
        var validator = new ExtendedValidator<TargetValueTypes>();

        var result = validator.Validate(new TargetValueTypes());
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
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
            .DontScrubGuids()
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Disabled_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithDisabled>();

        var target = new TargetWithDisabled();
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: NotNullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: Guid_Empty
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Disabled_Empty()
    {
        var validator = new ExtendedValidator<TargetWithDisabled>();

        var target = new TargetWithDisabled
        {
            NotNullable = Guid.Empty,
            Nullable = Guid.Empty
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: NotNullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: Guid_Empty
                      }
                    },
                    {
                      PropertyName: Nullable,
                      ErrorMessage: Nullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Nullable,
                        PropertyPath: Nullable,
                        PropertyValue: Guid_Empty
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

#nullable disable
    class TargetWithDisabled
    {
        public Guid? Nullable { get; set; }
        public Guid NotNullable { get; set; }
    }
#nullable enable
    [Test]
    public Task Dates_NonEmpty()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates
        {
            NotNullableDateTime = new(2000, 1, 1),
            NotNullableDateTimeOffset = new DateTime(2000, 1, 1),
            NotNullableDate = new(2000, 1, 1),
            NullableDateTime = new DateTime(2000, 1, 1),
            NullableDateTimeOffset = new DateTime(2000, 1, 1),
            NullableDate = new Date(2000, 1, 1),
            NotNullableAllowEmptyDateTime = new(2000, 1, 1),
            NotNullableAllowEmptyDateTimeOffset = new DateTime(2000, 1, 1),
            NotNullableAllowEmptyDate = new(2000, 1, 1),
            NullableAllowEmptyDateTime = new DateTime(2000, 1, 1),
            NullableAllowEmptyDateTimeOffset = new DateTime(2000, 1, 1),
            NullableAllowEmptyDate = new Date(2000, 1, 1)
        };
        var result = validator.Validate(target);
        return Verify(result)
            .DontScrubGuids()
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Dates_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates();
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullableDateTime,
                      ErrorMessage: NotNullableDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date Time,
                        PropertyPath: NotNullableDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDateTime,
                      ErrorMessage: NotNullableAllowEmptyDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date Time,
                        PropertyPath: NotNullableAllowEmptyDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableDateTimeOffset,
                      ErrorMessage: NotNullableDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date Time Offset,
                        PropertyPath: NotNullableDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDateTimeOffset,
                      ErrorMessage: NotNullableAllowEmptyDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date Time Offset,
                        PropertyPath: NotNullableAllowEmptyDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableDate,
                      ErrorMessage: NotNullableDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date,
                        PropertyPath: NotNullableDate,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDate,
                      ErrorMessage: NotNullableAllowEmptyDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date,
                        PropertyPath: NotNullableAllowEmptyDate,
                        PropertyValue: Date_MinValue
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Dates_Min()
    {
        var validator = new ExtendedValidator<TargetWithDates>();

        var target = new TargetWithDates
        {
            NotNullableDateTime = DateTime.MinValue,
            NotNullableDateTimeOffset = DateTimeOffset.MinValue,
            NotNullableDate = Date.MinValue,
            NullableDateTime = DateTime.MinValue,
            NullableDateTimeOffset = DateTimeOffset.MinValue,
            NullableDate = Date.MinValue,
            NotNullableAllowEmptyDateTime = DateTime.MinValue,
            NotNullableAllowEmptyDateTimeOffset = DateTimeOffset.MinValue,
            NotNullableAllowEmptyDate = Date.MinValue,
            NullableAllowEmptyDateTime = DateTime.MinValue,
            NullableAllowEmptyDateTimeOffset = DateTimeOffset.MinValue,
            NullableAllowEmptyDate = Date.MinValue
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullableDateTime,
                      ErrorMessage: NotNullableDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date Time,
                        PropertyPath: NotNullableDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDateTime,
                      ErrorMessage: NotNullableAllowEmptyDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date Time,
                        PropertyPath: NotNullableAllowEmptyDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableDateTime,
                      ErrorMessage: NullableDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Date Time,
                        PropertyPath: NullableDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableAllowEmptyDateTime,
                      ErrorMessage: NullableAllowEmptyDateTime must not be `DateTime.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Allow Empty Date Time,
                        PropertyPath: NullableAllowEmptyDateTime,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableDateTimeOffset,
                      ErrorMessage: NotNullableDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date Time Offset,
                        PropertyPath: NotNullableDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDateTimeOffset,
                      ErrorMessage: NotNullableAllowEmptyDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date Time Offset,
                        PropertyPath: NotNullableAllowEmptyDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableDateTimeOffset,
                      ErrorMessage: NullableDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Date Time Offset,
                        PropertyPath: NullableDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableAllowEmptyDateTimeOffset,
                      ErrorMessage: NullableAllowEmptyDateTimeOffset must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Allow Empty Date Time Offset,
                        PropertyPath: NullableAllowEmptyDateTimeOffset,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableDate,
                      ErrorMessage: NotNullableDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Date,
                        PropertyPath: NotNullableDate,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmptyDate,
                      ErrorMessage: NotNullableAllowEmptyDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Not Nullable Allow Empty Date,
                        PropertyPath: NotNullableAllowEmptyDate,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableDate,
                      ErrorMessage: NullableDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Date,
                        PropertyPath: NullableDate,
                        PropertyValue: Date_MinValue
                      }
                    },
                    {
                      PropertyName: NullableAllowEmptyDate,
                      ErrorMessage: NullableAllowEmptyDate must not be `DateOnly.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Nullable Allow Empty Date,
                        PropertyPath: NullableAllowEmptyDate,
                        PropertyValue: Date_MinValue
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    class TargetWithDates
    {
        public DateTime? NullableDateTime { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public Date? NullableDate { get; set; }
        public DateTime NotNullableDateTime { get; set; }
        public DateTimeOffset NotNullableDateTimeOffset { get; set; }
        public Date NotNullableDate { get; set; }

        [AllowEmpty]
        public DateTime? NullableAllowEmptyDateTime { get; set; }

        [AllowEmpty]
        public DateTimeOffset? NullableAllowEmptyDateTimeOffset { get; set; }

        [AllowEmpty]
        public Date? NullableAllowEmptyDate { get; set; }

        [AllowEmpty]
        public DateTime NotNullableAllowEmptyDateTime { get; set; }

        [AllowEmpty]
        public DateTimeOffset NotNullableAllowEmptyDateTimeOffset { get; set; }

        [AllowEmpty]
        public Date NotNullableAllowEmptyDate { get; set; }
    }

    [Test]
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
            .DontScrubGuids()
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Guids_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithGuids>();

        var target = new TargetWithGuids();
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: NotNullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: Guid_Empty
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: NotNullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: Guid_Empty
                      }
                    },
                    {
                      PropertyName: Nullable,
                      ErrorMessage: Nullable must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Nullable,
                        PropertyPath: Nullable,
                        PropertyValue: Guid_Empty
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task List_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithLists>();

        var target = new TargetWithLists();
        var result = validator.Validate(target);
        return Verify(result)
          .NotInline();
    }

    [Test]
    public Task List_Defaults_ValidateEmptyLists()
    {
        var validator = new ExtendedValidator<TargetWithLists>(validateEmptyLists: true);

        var target = new TargetWithLists();
        var result = validator.Validate(target);
        return Verify(result)
            .NotInline();
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: 'Not Nullable' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable
                      }
                    },
                    {
                      PropertyName: Nullable,
                      ErrorMessage: 'Nullable' must not be empty.,
                      ErrorCode: NotEmptyCollectionValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Nullable,
                        PropertyPath: Nullable
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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
        return Verify(result)
            .NotInline();
    }

    class TargetWithLists
    {
        public List<string>? Nullable { get; set; }
        public List<string> NotNullable { get; set; }
    }

    [Test]
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
    public Task Strings_Defaults()
    {
        var validator = new ExtendedValidator<TargetWithStrings>();

        var target = new TargetWithStrings();
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: 'Not Nullable' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: null
                      }
                    },
                    {
                      PropertyName: NotNullableAllowEmpty,
                      ErrorMessage: 'Not Nullable Allow Empty' must not be null.,
                      ErrorCode: NotNullValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Not Nullable Allow Empty,
                        PropertyPath: NotNullableAllowEmpty,
                        PropertyValue: null
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: 'Not Nullable' must not be empty.,
                      AttemptedValue: ,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue: 
                      }
                    },
                    {
                      PropertyName: Nullable,
                      ErrorMessage: 'Nullable' must not be whitespace.,
                      AttemptedValue: ,
                      ErrorCode: NotWhiteSpaceValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Nullable,
                        PropertyPath: Nullable,
                        PropertyValue: 
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    [Test]
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
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: NotNullable,
                      ErrorMessage: 'Not Nullable' must not be empty.,
                      AttemptedValue:  ,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Not Nullable,
                        PropertyPath: NotNullable,
                        PropertyValue:  
                      }
                    },
                    {
                      PropertyName: Nullable,
                      ErrorMessage: 'Nullable' must not be whitespace.,
                      AttemptedValue:  ,
                      ErrorCode: NotWhiteSpaceValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Nullable,
                        PropertyPath: Nullable,
                        PropertyValue:  
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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

    [Test]
    public Task Compounded()
    {
        var validator = new TargetCompoundedValidator();

        var target = new TargetCompounded
        {
            Property1 = null!,
            Property2 = "123"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: Property1,
                      ErrorMessage: 'Property1' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Property1,
                        PropertyPath: Property1,
                        PropertyValue: null
                      }
                    },
                    {
                      PropertyName: Property2,
                      ErrorMessage: The length of 'Property2' must be 2 characters or fewer. You entered 3 characters.,
                      AttemptedValue: 123,
                      ErrorCode: MaximumLengthValidator,
                      FormattedMessagePlaceholderValues: {
                        MaxLength: 2,
                        MinLength: 0,
                        PropertyName: Property2,
                        PropertyPath: Property2,
                        PropertyValue: 123,
                        TotalLength: 3
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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

    [Test]
    public Task Newlines()
    {
        var validator = new TargetWithStringPropertiesValidator();

        var target = new TargetWithStringProperties
        {
            EmptyString = "",
            WhitespaceString = " ",
            RNString = "\r\n",
            WrappedRNString = "a\r\nb",
            NString = "\n",
            WrappedNString = "a\nb",
            RString = "\r",
            WrappedRString = "a\rb",
            ValidString = "ab"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: RNString,
                      ErrorMessage: 'RN String' must not contain new line characters.,
                      AttemptedValue:
                ,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: RN String,
                        PropertyPath: RNString,
                        PropertyValue:

                      }
                    },
                    {
                      PropertyName: WrappedRNString,
                      ErrorMessage: 'Wrapped RN String' must not contain new line characters.,
                      AttemptedValue:
                a
                b,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Wrapped RN String,
                        PropertyPath: WrappedRNString,
                        PropertyValue:
                a
                b
                      }
                    },
                    {
                      PropertyName: NString,
                      ErrorMessage: 'N String' must not contain new line characters.,
                      AttemptedValue:
                ,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: N String,
                        PropertyPath: NString,
                        PropertyValue:

                      }
                    },
                    {
                      PropertyName: WrappedNString,
                      ErrorMessage: 'Wrapped N String' must not contain new line characters.,
                      AttemptedValue:
                a
                b,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Wrapped N String,
                        PropertyPath: WrappedNString,
                        PropertyValue:
                a
                b
                      }
                    },
                    {
                      PropertyName: RString,
                      ErrorMessage: 'R String' must not contain new line characters.,
                      AttemptedValue:
                ,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: R String,
                        PropertyPath: RString,
                        PropertyValue:

                      }
                    },
                    {
                      PropertyName: WrappedRString,
                      ErrorMessage: 'Wrapped R String' must not contain new line characters.,
                      AttemptedValue:
                a
                b,
                      ErrorCode: NotContainNewlineValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Wrapped R String,
                        PropertyPath: WrappedRString,
                        PropertyValue:
                a
                b
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    public class TargetWithStringProperties
    {
        public string? NullString { get; set; }
        public string WhitespaceString { get; set; } = null!;
        public string EmptyString { get; set; } = null!;
        public string RNString { get; set; } = null!;
        public string WrappedRNString { get; set; } = null!;
        public string NString { get; set; } = null!;
        public string WrappedNString { get; set; } = null!;
        public string RString { get; set; } = null!;
        public string WrappedRString { get; set; } = null!;
        public string ValidString { get; set; } = null!;
    }

    class TargetWithStringPropertiesValidator :
        AbstractValidator<TargetWithStringProperties>
    {
        public TargetWithStringPropertiesValidator()
        {
            RuleFor(_ => _.NullString).NotContainNewlines();
            RuleFor(_ => _.WhitespaceString).NotContainNewlines();
            RuleFor(_ => _.EmptyString).NotContainNewlines();
            RuleFor(_ => _.RNString).NotContainNewlines();
            RuleFor(_ => _.WrappedRNString).NotContainNewlines();
            RuleFor(_ => _.NString).NotContainNewlines();
            RuleFor(_ => _.WrappedNString).NotContainNewlines();
            RuleFor(_ => _.RString).NotContainNewlines();
            RuleFor(_ => _.WrappedRString).NotContainNewlines();
            RuleFor(_ => _.ValidString).NotContainNewlines();
        }
    }

    [Test]
    public Task ValueTypes_WithValues()
    {
        var validator = new ExtendedValidator<TargetValueTypes>();

        var target = new TargetValueTypes
        {
            NotNullable = true,
            Nullable = true,
        };
        var result = validator.Validate(target);
        return Verify(result)
            .Snapshot(
                """
                {
                  IsValid: true,
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
    }

    class TargetValueTypes
    {
        public bool? Nullable { get; set; }
        public bool NotNullable { get; set; }
    }

    [Test]
    public Task Usage()
    {
        var validator = new PersonValidatorFromBase();

        var target = new Person
        {
            FirstName = "Joe"
        };
        var result = validator.Validate(target);
        return Verify(result)
            .ScrubReplace("1/1/0001", "1/01/0001")
            .Snapshot(
                """
                {
                  IsValid: false,
                  Errors: [
                    {
                      PropertyName: FamilyName,
                      ErrorMessage: 'Family Name' must not be empty.,
                      ErrorCode: NotEmptyValidator,
                      FormattedMessagePlaceholderValues: {
                        PropertyName: Family Name,
                        PropertyPath: FamilyName,
                        PropertyValue: null
                      }
                    },
                    {
                      PropertyName: Id,
                      ErrorMessage: Id must not be `Guid.Empty`.,
                      AttemptedValue: Guid_Empty,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Guid_Empty,
                        PropertyName: Id,
                        PropertyPath: Id,
                        PropertyValue: Guid_Empty
                      }
                    },
                    {
                      PropertyName: Dob,
                      ErrorMessage: Dob must not be `DateTimeOffset.MinValue`.,
                      AttemptedValue: Date_MinValue,
                      ErrorCode: NotEqualValidator,
                      FormattedMessagePlaceholderValues: {
                        ComparisonProperty: ,
                        ComparisonValue: Date_MinValue,
                        PropertyName: Dob,
                        PropertyPath: Dob,
                        PropertyValue: Date_MinValue
                      }
                    }
                  ],
                  RuleSetsExecuted: [
                    default
                  ]
                }
                """);
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

    // ReSharper disable once EmptyConstructor

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