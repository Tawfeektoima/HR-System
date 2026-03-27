-- =======================================================
-- Migration 003: Create Users Table (HR Staff)
-- =======================================================

CREATE TABLE Users (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    FirstName       NVARCHAR(100)   NOT NULL,
    LastName        NVARCHAR(100)   NOT NULL,
    Email           NVARCHAR(255)   NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(500)   NOT NULL,
    Role            NVARCHAR(20)    NOT NULL DEFAULT 'HR'
                    CHECK (Role IN ('Admin', 'HR', 'Interviewer')),
    IsActive        BIT             NOT NULL DEFAULT 1,
    LastLoginAt     DATETIME2       NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Users_Email ON Users(Email);

-- Now add FK on Jobs table
ALTER TABLE Jobs
    ADD CONSTRAINT FK_Jobs_CreatedBy
    FOREIGN KEY (CreatedById) REFERENCES Users(Id);

PRINT 'Table [Users] created successfully.';
