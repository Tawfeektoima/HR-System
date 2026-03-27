using FluentValidation;
using HRSystem.Core.DTOs.Interview;

namespace HRSystem.API.Validators;

public class CreateInterviewValidator : AbstractValidator<CreateInterviewDto>
{
    public CreateInterviewValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0).WithMessage("Valid ApplicationId is required.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Interview must be scheduled in the future.");

        RuleFor(x => x.Type)
            .Must(t => t == "Phone" || t == "Technical" || t == "HR" || t == "Final")
            .WithMessage("Invalid interview type.");
            
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.")
            .LessThanOrEqualTo(240).WithMessage("Duration cannot exceed 4 hours.");
            
        RuleFor(x => x.Location)
            .NotEmpty()
            .When(x => !x.IsOnline)
            .WithMessage("Location is required for required physical interviews.");
    }
}
