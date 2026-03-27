-- =======================================================
-- Migration 006: Create Skills Table
-- =======================================================

CREATE TABLE Skills (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    CandidateId     INT             NOT NULL,
    SkillName       NVARCHAR(100)   NOT NULL,
    Level           NVARCHAR(20)    NULL
                    CHECK (Level IN ('Beginner','Intermediate','Senior','Expert') OR Level IS NULL),
    Source          NVARCHAR(20)    NOT NULL DEFAULT 'AI'
                    CHECK (Source IN ('AI','Manual')),
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Skills_Candidate FOREIGN KEY (CandidateId) REFERENCES Candidates(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Skills_CandidateId ON Skills(CandidateId);
CREATE INDEX IX_Skills_SkillName   ON Skills(SkillName);

-- =======================================================
-- Migration 007: Create Notifications Table
-- =======================================================

CREATE TABLE Notifications (
    Id              INT             IDENTITY(1,1)   PRIMARY KEY,
    UserId          INT             NOT NULL,
    Title           NVARCHAR(200)   NOT NULL,
    Message         NVARCHAR(1000)  NOT NULL,
    Type            NVARCHAR(30)    NOT NULL DEFAULT 'Info'
                    CHECK (Type IN ('Info','Warning','Success','Error')),
    IsRead          BIT             NOT NULL DEFAULT 0,
    RelatedEntityId INT             NULL,
    RelatedEntity   NVARCHAR(50)    NULL,  -- 'Application' | 'Interview' | 'Job'
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Notifications_UserId  ON Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead  ON Notifications(IsRead);

PRINT 'Tables [Skills] and [Notifications] created successfully.';
