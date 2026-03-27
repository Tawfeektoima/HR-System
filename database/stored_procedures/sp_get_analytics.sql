-- =======================================================
-- Stored Procedure: Get Dashboard Analytics
-- =======================================================

CREATE OR ALTER PROCEDURE sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM Jobs   WHERE Status = 'Open')       AS OpenJobs,
        (SELECT COUNT(*) FROM Jobs)                                AS TotalJobs,
        (SELECT COUNT(*) FROM Candidates)                          AS TotalCandidates,
        (SELECT COUNT(*) FROM Applications)                        AS TotalApplications,
        (SELECT COUNT(*) FROM Applications WHERE Status = 'Accepted')  AS TotalAccepted,
        (SELECT COUNT(*) FROM Applications WHERE Status = 'Rejected')  AS TotalRejected,
        (SELECT COUNT(*) FROM Interviews  WHERE ScheduledAt >= GETUTCDATE()) AS UpcomingInterviews,
        CAST(
            CASE WHEN (SELECT COUNT(*) FROM Applications) = 0 THEN 0
            ELSE (SELECT COUNT(*) FROM Applications WHERE Status='Accepted') * 100.0
                 / (SELECT COUNT(*) FROM Applications WHERE Status IN ('Accepted','Rejected'))
            END AS DECIMAL(5,2)
        ) AS AcceptanceRate,
        CAST(
            ISNULL(AVG(DATEDIFF(DAY, a.AppliedAt,
                CASE WHEN a.Status='Accepted' THEN a.UpdatedAt END)),0)
        AS DECIMAL(8,2)) AS AvgDaysToHire
    FROM Applications a;
END;
GO

-- =======================================================
-- Stored Procedure: Applications Per Month (Chart Data)
-- =======================================================
CREATE OR ALTER PROCEDURE sp_GetApplicationsPerMonth
    @MonthsBack INT = 6
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FORMAT(AppliedAt, 'yyyy-MM') AS MonthLabel,
        COUNT(*)                      AS TotalApplications,
        SUM(CASE WHEN Status='Accepted' THEN 1 ELSE 0 END) AS Accepted,
        SUM(CASE WHEN Status='Rejected' THEN 1 ELSE 0 END) AS Rejected
    FROM Applications
    WHERE AppliedAt >= DATEADD(MONTH, -@MonthsBack, GETUTCDATE())
    GROUP BY FORMAT(AppliedAt, 'yyyy-MM')
    ORDER BY MonthLabel;
END;
GO

-- =======================================================
-- Stored Procedure: ATS Pipeline Funnel
-- =======================================================
CREATE OR ALTER PROCEDURE sp_GetPipelineFunnel
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Status,
        COUNT(*) AS Count,
        CAST(COUNT(*) * 100.0 / NULLIF((SELECT COUNT(*) FROM Applications),0) AS DECIMAL(5,2)) AS Percentage
    FROM Applications
    GROUP BY Status
    ORDER BY
        CASE Status
            WHEN 'Applied'              THEN 1
            WHEN 'PhoneInterview'       THEN 2
            WHEN 'TechnicalInterview'   THEN 3
            WHEN 'FinalInterview'       THEN 4
            WHEN 'Accepted'             THEN 5
            WHEN 'Rejected'             THEN 6
        END;
END;
GO

-- =======================================================
-- Stored Procedure: Top Jobs By Applications
-- =======================================================
CREATE OR ALTER PROCEDURE sp_GetTopJobsByApplications
    @TopN INT = 5
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        j.Id,
        j.Title,
        j.Department,
        COUNT(a.Id) AS ApplicationCount,
        AVG(a.CvScore) AS AvgScore
    FROM Jobs j
    LEFT JOIN Applications a ON a.JobId = j.Id
    GROUP BY j.Id, j.Title, j.Department
    ORDER BY ApplicationCount DESC;
END;
GO

PRINT 'All stored procedures created successfully.';
