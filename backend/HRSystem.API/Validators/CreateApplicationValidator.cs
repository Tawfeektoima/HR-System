using FluentValidation;
using HRSystem.Core.DTOs.Application;

namespace HRSystem.API.Validators;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationDto>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("Valid JobId is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name too long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name too long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.LinkedInUrl)
            .Matches(@"^https?:\/\/(www\.)?linkedin\.com\/.*$")
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Must be a valid LinkedIn URL.");
    }
}
