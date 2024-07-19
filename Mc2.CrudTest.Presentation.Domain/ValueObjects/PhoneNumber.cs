using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using PhoneNumbers;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }
    public PhoneNumber() { }

    private PhoneNumber(string value)
    {
        Value = value.Replace(" ", "");
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        PhoneNumberValidator validator = new PhoneNumberValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(phoneNumber);
        if (!validationResult.IsValid)
            return Result<PhoneNumber>.Invalid(validationResult.AsErrors());
        return Result<PhoneNumber>.Success(new PhoneNumber(phoneNumber));
    }

    public override string ToString() => Value;
}
