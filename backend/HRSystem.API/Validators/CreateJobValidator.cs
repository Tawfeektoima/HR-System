using FluentValidation;
using HRSystem.Core.DTOs.Job;

namespace HRSystem.API.Validators;

public class CreateJobValidator : AbstractValidator<CreateJobDto>
{
    public CreateJobValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required.");

        RuleFor(x => x.Requirements)
            .NotEmpty().WithMessage("Job requirements are required.");

        RuleFor(x => x.SalaryMax)
            .GreaterThan(x => x.SalaryMin).When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue)
            .WithMessage("Maximum salary must be greater than minimum salary.");
            
        RuleFor(x => x.DeadlineAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.DeadlineAt.HasValue)
            .WithMessage("Deadline must be in the future.");
    }
}
