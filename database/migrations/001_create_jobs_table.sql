-- =======================================================
-- Migration 001: Create Jobs Table
-- =======================================================

CREATE TABLE Jobs (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    Title           NVARCHAR(200)   NOT NULL,
    Department      NVARCHAR(100)   NOT NULL,
    Description     NVARCHAR(MAX)   NOT NULL,
    Requirements    NVARCHAR(MAX)   NOT NULL,
    SalaryMin       DECIMAL(10,2)   NULL,
    SalaryMax       DECIMAL(10,2)   NULL,
    Location        NVARCHAR(200)   NULL,
    IsRemote        BIT             NOT NULL DEFAULT 0,
    Status          NVARCHAR(20)    NOT NULL DEFAULT 'Open'  -- Open | Closed | Draft
                    CHECK (Status IN ('Open', 'Closed', 'Draft')),
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    DeadlineAt      DATETIME2       NULL,
    CreatedById     INT             NULL   -- FK to Users
);

-- Index for fast search by Status
CREATE INDEX IX_Jobs_Status ON Jobs(Status);

-- Index for search by Department
CREATE INDEX IX_Jobs_Department ON Jobs(Department);

PRINT 'Table [Jobs] created successfully.';
