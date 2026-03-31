using HRSystem.API.Extensions;
using Serilog;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR System API", Version = "v1" });
    
    // JWT configuration in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Use our extension methods to clean up Program.cs
builder.Services.AddDatabase(builder.Configuration)
                .AddRepositories()
                .AddServices()
                .AddJwtAuthentication(builder.Configuration)
                .AddValidations();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<HRSystem.API.Middleware.ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

// ============================================
// Auto-Migration and Data Seeding
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HRSystem.Infrastructure.Data.HRSystemDbContext>();
        context.Database.EnsureCreated(); // Creates DB if not exists (simpler than migration for quick demo)
        
        // ============================================
        // Seed HR Admin User
        // ============================================
        if (!context.Users.Any(u => u.Email == "admin@hr.com"))
        {
            var admin = new HRSystem.Core.Entities.User
            {
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@hr.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                Role = HRSystem.Core.Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(admin);
            context.SaveChanges();
        }

        // ============================================
        // Seed Jobs
        // ============================================
        if (!context.Jobs.Any())
        {
            var jobs = new List<HRSystem.Core.Entities.Job>
            {
                new HRSystem.Core.Entities.Job { 
                    Title = "Call Center Representative (Fluent English)", 
                    Department = "Customer Operations", 
                    Description = "Handle inbound calls from US/UK customers. Requires near-native English fluency and excellent communication.",
                    Requirements = "Fluent English (C1/C2), Prior experience in CS, Shift flexibility.",
                    Location = "Cairo, Egypt",
                    IsRemote = false,
                    Status = HRSystem.Core.Enums.JobStatus.Open
                },
                new HRSystem.Core.Entities.Job { 
                    Title = "Senior Full Stack Developer (.NET/React)", 
                    Department = "Engineering", 
                    Description = "Build and maintain our core HR platform.",
                    Requirements = "5+ years C#, React, SQL Server/SQLite, Docker.",
                    Location = "Dubai, UAE",
                    IsRemote = true,
                    Status = HRSystem.Core.Enums.JobStatus.Open
                },
                new HRSystem.Core.Entities.Job { 
                    Title = "Social Media Moderator", 
                    Department = "Marketing", 
                    Description = "Moderate comments and interact with users on Facebook and Instagram.",
                    Requirements = "Arabic/English fluency, Creative writing skills.",
                    Location = "Riyadh, SA",
                    IsRemote = true,
                    Status = HRSystem.Core.Enums.JobStatus.Closed
                }
            };
            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }

        // ============================================
        // Seed Candidates & Applications
        // ============================================
        if (!context.Applications.Any())
        {
            var callCenterJob = context.Jobs.First(j => j.Title.Contains("Call Center"));
            var devJob = context.Jobs.First(j => j.Title.Contains("Full Stack"));

            var candidates = new List<HRSystem.Core.Entities.Candidate>
            {
                new HRSystem.Core.Entities.Candidate { FirstName = "Ahmed", LastName = "Ali", Email = "ahmed.ali@example.com", ExperienceYears = 2, EducationLevel = "Bachelor of Commerce", Phone = "01012345678" },
                new HRSystem.Core.Entities.Candidate { FirstName = "Sarah", LastName = "Johnson", Email = "sarah.j@example.com", ExperienceYears = 4, EducationLevel = "Bachelor of Arts", Phone = "01187654321" },
                new HRSystem.Core.Entities.Candidate { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", ExperienceYears = 1, EducationLevel = "High School Diploma", Phone = "01234567890" },
                new HRSystem.Core.Entities.Candidate { FirstName = "Mona", LastName = "Sami", Email = "mona.s@example.com", ExperienceYears = 3, EducationLevel = "Bachelor of Science", Phone = "01598765432" },
                new HRSystem.Core.Entities.Candidate { FirstName = "Rami", LastName = "Fares", Email = "rami.f@example.com", ExperienceYears = 6, EducationLevel = "Master of IT", Phone = "01000001234" }
            };

            context.Candidates.AddRange(candidates);
            context.SaveChanges();

            var applications = new List<HRSystem.Core.Entities.Application>
            {
                // Call Center Applications
                new HRSystem.Core.Entities.Application { CandidateId = candidates[0].Id, JobId = callCenterJob.Id, Status = HRSystem.Core.Enums.ApplicationStatus.Applied, CvScore = 85, AppliedAt = DateTime.UtcNow.AddDays(-2) },
                new HRSystem.Core.Entities.Application { CandidateId = candidates[1].Id, JobId = callCenterJob.Id, Status = HRSystem.Core.Enums.ApplicationStatus.PhoneInterview, CvScore = 92, AppliedAt = DateTime.UtcNow.AddDays(-5) },
                new HRSystem.Core.Entities.Application { CandidateId = candidates[2].Id, JobId = callCenterJob.Id, Status = HRSystem.Core.Enums.ApplicationStatus.Rejected, CvScore = 45, AppliedAt = DateTime.UtcNow.AddDays(-10) },
                
                // Dev Applications
                new HRSystem.Core.Entities.Application { CandidateId = candidates[3].Id, JobId = devJob.Id, Status = HRSystem.Core.Enums.ApplicationStatus.TechnicalInterview, CvScore = 78, AppliedAt = DateTime.UtcNow.AddDays(-1) },
                new HRSystem.Core.Entities.Application { CandidateId = candidates[4].Id, JobId = devJob.Id, Status = HRSystem.Core.Enums.ApplicationStatus.Accepted, CvScore = 95, AppliedAt = DateTime.UtcNow.AddDays(-15) }
            };

            context.Applications.AddRange(applications);
            context.SaveChanges();

            // Link CV placeholder file paths
            foreach (var application in applications)
            {
                var candidate = candidates.First(c => c.Id == application.CandidateId);
                candidate.CvFilePath = $"uploads/cvs/cv_{candidate.FirstName.ToLower()}.txt";
            }
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
// ============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files for CVs (if local storage is used)
app.UseStaticFiles();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
