using FluentValidation;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
public class PhoneNumberValidator : AbstractValidator<string>
{
    public PhoneNumberValidator()
    {
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        RuleFor(phoneNumber => phoneNumber)
            .Must(phoneNumber =>
            {
                try
                {
                    var parsedPhoneNumber = phoneNumberUtil.Parse(phoneNumber, null);
                    return phoneNumberUtil.IsValidNumber(parsedPhoneNumber);
                }
                catch
                {
                    return false;
                }
            }).WithMessage("Mobile Number is not valid");
    }
}
