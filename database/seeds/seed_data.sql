-- =======================================================
-- Seed Data — بيانات تجريبية للتطوير
-- =======================================================

-- Admin User (Password: Admin@123 — BCrypt hash)
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role) VALUES
('Ahmed',   'Hassan',  'admin@hr-system.com',       '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TiGc2L2JZG6N8mKzXpKqvBHFzH9a', 'Admin'),
('Sara',    'Mostafa', 'sara.hr@hr-system.com',      '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TiGc2L2JZG6N8mKzXpKqvBHFzH9a', 'HR'),
('Mohamed', 'Ali',     'mohamed.hr@hr-system.com',   '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TiGc2L2JZG6N8mKzXpKqvBHFzH9a', 'HR'),
('Layla',   'Ibrahim', 'layla.int@hr-system.com',    '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TiGc2L2JZG6N8mKzXpKqvBHFzH9a', 'Interviewer');

-- Jobs
INSERT INTO Jobs (Title, Department, Description, Requirements, SalaryMin, SalaryMax, Location, IsRemote, Status, DeadlineAt, CreatedById) VALUES
('Senior Backend Developer',   'Engineering', 
 'We are looking for a Senior Backend Developer to join our growing engineering team. You will design and build scalable APIs and microservices.',
 'C#, ASP.NET Core, SQL Server, 5+ years experience, REST APIs, Docker', 
 15000, 25000, 'Cairo, Egypt', 0, 'Open', DATEADD(DAY, 30, GETUTCDATE()), 1),

('React Frontend Developer',   'Engineering',
 'Join our frontend team to build modern, responsive web interfaces used by thousands of users daily.',
 'React.js, TypeScript, CSS, REST APIs, 3+ years experience',
 10000, 18000, 'Remote', 1, 'Open', DATEADD(DAY, 45, GETUTCDATE()), 2),

('Data Scientist',             'AI & Data',
 'Work on cutting-edge machine learning models to solve real business problems.',
 'Python, TensorFlow/PyTorch, Scikit-learn, SQL, 4+ years experience',
 18000, 30000, 'Cairo, Egypt', 1, 'Open', DATEADD(DAY, 20, GETUTCDATE()), 1),

('HR Business Partner',        'Human Resources',
 'Support business units with talent management, recruitment, and HR policies.',
 'HR Certification, 3+ years experience, Excellent communication skills',
 8000, 14000, 'Alexandria, Egypt', 0, 'Open', DATEADD(DAY, 60, GETUTCDATE()), 2),

('DevOps Engineer',            'Infrastructure',
 'Maintain and improve CI/CD pipelines, cloud infrastructure, and monitoring systems.',
 'Docker, Kubernetes, AWS/Azure, CI/CD, Linux, 4+ years',
 14000, 22000, 'Remote', 1, 'Closed', DATEADD(DAY,-5, GETUTCDATE()), 1);

-- Candidates
INSERT INTO Candidates (FirstName, LastName, Email, Phone, LinkedInUrl, ExperienceYears, EducationLevel, TotalScore) VALUES
('Khaled',  'Mahmoud',  'khaled.m@email.com',    '01012345678', 'linkedin.com/in/khaledm',  6, 'Bachelor''s in Computer Science', 87.5),
('Nour',    'Ahmed',    'nour.a@email.com',       '01123456789', 'linkedin.com/in/noura',    3, 'Bachelor''s in Software Engineering', 72.0),
('Omar',    'Fathi',    'omar.f@email.com',       '01234567890', 'linkedin.com/in/omarf',    8, 'Master''s in Computer Science', 91.2),
('Dina',    'Hassan',   'dina.h@email.com',       '01112233445', NULL,                       2, 'Bachelor''s in Information Systems', 54.3),
('Youssef', 'Samir',    'youssef.s@email.com',    '01099887766', 'linkedin.com/in/youssefs', 5, 'Bachelor''s in Computer Engineering', 79.8);

-- Skills
INSERT INTO Skills (CandidateId, SkillName, Level, Source) VALUES
(1,'C#','Senior','AI'),        (1,'ASP.NET Core','Senior','AI'),  (1,'SQL Server','Intermediate','AI'),
(1,'Docker','Intermediate','AI'),(1,'Git','Senior','AI'),
(2,'React.js','Senior','AI'),  (2,'TypeScript','Intermediate','AI'),(2,'CSS','Senior','AI'),
(2,'Node.js','Beginner','AI'), (2,'Figma','Intermediate','Manual'),
(3,'Python','Expert','AI'),    (3,'TensorFlow','Senior','AI'),    (3,'Scikit-learn','Expert','AI'),
(3,'SQL','Senior','AI'),       (3,'Docker','Intermediate','AI'),
(4,'Java','Beginner','AI'),    (4,'SQL','Intermediate','AI'),
(5,'C#','Intermediate','AI'),  (5,'ASP.NET Core','Intermediate','AI'),(5,'Azure','Beginner','AI'),
(5,'SQL Server','Senior','AI'),(5,'React.js','Beginner','AI');

-- Applications
INSERT INTO Applications (CandidateId, JobId, Status, CvScore) VALUES
(1, 1, 'TechnicalInterview', 87.5),
(2, 2, 'PhoneInterview',     72.0),
(3, 3, 'FinalInterview',     91.2),
(4, 1, 'Applied',            54.3),
(5, 1, 'PhoneInterview',     79.8),
(3, 1, 'Accepted',           91.2),
(2, 4, 'Rejected',           45.0);

PRINT 'Seed data inserted successfully.';
