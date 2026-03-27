-- =======================================================
-- Migration 002: Create Candidates Table
-- =======================================================

CREATE TABLE Candidates (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    FirstName       NVARCHAR(100)   NOT NULL,
    LastName        NVARCHAR(100)   NOT NULL,
    Email           NVARCHAR(255)   NOT NULL UNIQUE,
    Phone           NVARCHAR(20)    NULL,
    LinkedInUrl     NVARCHAR(500)   NULL,
    PortfolioUrl    NVARCHAR(500)   NULL,
    CvFilePath      NVARCHAR(1000)  NULL,
    TotalScore      DECIMAL(5,2)    NULL DEFAULT 0, -- AI Score 0-100
    ExperienceYears INT             NULL DEFAULT 0,
    EducationLevel  NVARCHAR(100)   NULL,
    AiSummary       NVARCHAR(MAX)   NULL, -- AI-generated summary
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Candidates_Email ON Candidates(Email);
CREATE INDEX IX_Candidates_TotalScore ON Candidates(TotalScore DESC);

PRINT 'Table [Candidates] created successfully.';
