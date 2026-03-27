-- =======================================================
-- Migration 005: Create Interviews Table
-- =======================================================

CREATE TABLE Interviews (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    ApplicationId   INT             NOT NULL,
    InterviewerId   INT             NULL,
    Type            NVARCHAR(30)    NOT NULL
                    CHECK (Type IN ('Phone','Technical','Final','HR')),
    ScheduledAt     DATETIME2       NOT NULL,
    DurationMinutes INT             NOT NULL DEFAULT 60,
    Location        NVARCHAR(500)   NULL,   -- Physical location or video link
    IsOnline        BIT             NOT NULL DEFAULT 1,
    Result          NVARCHAR(20)    NULL
                    CHECK (Result IN ('Passed','Failed','NoShow','Pending') OR Result IS NULL),
    Score           DECIMAL(5,2)    NULL,
    Notes           NVARCHAR(MAX)   NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Interviews_Application FOREIGN KEY (ApplicationId) REFERENCES Applications(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Interviews_Interviewer FOREIGN KEY (InterviewerId)  REFERENCES Users(Id)
);

CREATE INDEX IX_Interviews_ApplicationId ON Interviews(ApplicationId);
CREATE INDEX IX_Interviews_ScheduledAt   ON Interviews(ScheduledAt);
CREATE INDEX IX_Interviews_InterviewerId ON Interviews(InterviewerId);

PRINT 'Table [Interviews] created successfully.';
