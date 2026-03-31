import sqlite3
import os
import uuid
from datetime import datetime

# Adjusting paths for Windows environment
base_path = r"d:\try\HR-System"
db_path = os.path.join(base_path, "backend", "HRSystem.API", "HRSystem.db")
cv_dir = os.path.join(base_path, "backend", "HRSystem.API", "wwwroot", "cvs")

if not os.path.exists(cv_dir):
    os.makedirs(cv_dir)

candidates_data = [
    ("Layla", "Hassan", "layla.h@example.com", "Near-native English speaker with 5 years in international call centers. Expert in conflict resolution.", 5, "Bachelor of Arts"),
    ("Omar", "Khaled", "omar.k@example.com", "Fresh graduate with C1 English. Enthusiastic about customer success. Fast learner.", 0, "Bachelor of Commerce"),
    ("Samantha", "Green", "sam.g@example.com", "Native speaker from California. 10 years experience in hospitality and customer relations.", 10, "High School Diploma"),
    ("Tamer", "Saad", "tamer.s@example.com", "Arabic speaker with basic English. 2 years experience in local retail stores.", 2, "Bachelor of Law"),
    ("Monica", "Bell", "monica.b@example.com", "Fluent English & French. 4 years at Vodafone UK account. High NPS scores.", 4, "Bachelor of Languages"),
    ("Kareem", "Zaid", "kareem.z@example.com", "Tech support specialist with 3 years experience. Fluent in English and German.", 3, "B.Sc. Computer Science"),
    ("Sara", "Mostafa", "sara.m@example.com", "Excellent communication skills. Fluent English. Previous experience at Etisalat.", 2, "Bachelor of Tourism"),
    ("John", "Smith", "john.smith@example.com", "Expert in data entry and administration. Limited experience in phone support.", 1, "Bachelor of Arts"),
    ("Hoda", "Kamal", "hoda.k@example.com", "Fluent English speaker. Master's in Communication. Interested in part-time roles.", 0, "Master of Arts"),
    ("Michael", "Jordan", "michael.j@example.com", "Highly competitive and results-oriented. 5 years in high-volume outbound sales.", 5, "Bachelor of Marketing")
]

job_id = 1 # Call Center

try:
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()

    now = "2026-03-31T03:42:00Z"

    for first, last, email, bio, exp, edu in candidates_data:
        # Check if email exists
        cursor.execute("SELECT Id FROM Candidates WHERE Email = ?", (email,))
        if cursor.fetchone():
            continue
            
        # 1. Create file
        filename = f"cv_{first.lower()}_{uuid.uuid4().hex[:8]}.txt"
        file_db_path = f"/cvs/{filename}"
        file_abs_path = os.path.join(cv_dir, filename)
        
        with open(file_abs_path, "w", encoding="utf-8") as f:
            f.write(f"CV for {first} {last}\nEmail: {email}\n\nSummary:\n{bio}\n\nExperience: {exp} years\nEducation: {edu}")
        
        # 2. Insert Candidate
        cursor.execute("""
            INSERT INTO Candidates (FirstName, LastName, Email, ExperienceYears, EducationLevel, CvFilePath, CreatedAt, UpdatedAt, TotalScore)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (first, last, email, exp, edu, file_db_path, now, now, 0))
        
        candidate_id = cursor.lastrowid
        
        # 3. Insert Application (Status 0 = Applied)
        cursor.execute("""
            INSERT INTO Applications (CandidateId, JobId, Status, CvScore, AppliedAt, UpdatedAt)
            VALUES (?, ?, ?, ?, ?, ?)
        """, (candidate_id, job_id, 0, 0, now, now))

    conn.commit()
    conn.close()
    print(f"Successfully added 10 candidates and linked them to Job ID {job_id}")
except Exception as e:
    print(f"Error: {e}")
