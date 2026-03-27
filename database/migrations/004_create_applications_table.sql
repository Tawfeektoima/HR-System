-- =======================================================
-- Migration 004: Create Applications Table (ATS Core)
-- =======================================================

CREATE TABLE Applications (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    CandidateId     INT             NOT NULL,
    JobId           INT             NOT NULL,
    Status          NVARCHAR(30)    NOT NULL DEFAULT 'Applied'
                    CHECK (Status IN ('Applied','PhoneInterview','TechnicalInterview','FinalInterview','Accepted','Rejected')),
    CvScore         DECIMAL(5,2)    NULL DEFAULT 0,
    HrNotes         NVARCHAR(MAX)   NULL,
    RejectionReason NVARCHAR(500)   NULL,
    AppliedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    ReviewedById    INT             NULL,

    CONSTRAINT FK_Applications_Candidate FOREIGN KEY (CandidateId) REFERENCES Candidates(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Applications_Job       FOREIGN KEY (JobId)        REFERENCES Jobs(Id)       ON DELETE CASCADE,
    CONSTRAINT FK_Applications_Reviewer  FOREIGN KEY (ReviewedById) REFERENCES Users(Id),
    CONSTRAINT UQ_Application_Candidate_Job UNIQUE (CandidateId, JobId)
);

CREATE INDEX IX_Applications_Status    ON Applications(Status);
CREATE INDEX IX_Applications_JobId     ON Applications(JobId);
CREATE INDEX IX_Applications_CandidateId ON Applications(CandidateId);
CREATE INDEX IX_Applications_AppliedAt ON Applications(AppliedAt DESC);

PRINT 'Table [Applications] created successfully.';
