from fastapi import APIRouter, UploadFile, File, Form, HTTPException
from typing import List, Optional
from pydantic import BaseModel
import os

from app.services.cv_parser import parse_cv
from app.services.skill_extractor import extract_skills, extract_experience_years, extract_education
from app.services.scoring_engine import calculate_score

router = APIRouter()

class CVAnalysisResponse(BaseModel):
    extracted_skills: List[str]
    experience_years: int
    education_level: str
    score: float
    summary: str

@router.post("/analyze-cv", response_model=CVAnalysisResponse)
async def analyze_cv(
    file: UploadFile = File(...),
    job_id: int = Form(...),
    job_requirements: Optional[str] = Form(None)
):
    temp_path = ""
    try:
        # Save file to a temporary location
        temp_dir = "/tmp" if os.name != 'nt' else os.getenv("TEMP", "C:/Temp")
        os.makedirs(temp_dir, exist_ok=True)
        temp_path = os.path.join(temp_dir, file.filename)
        
        with open(temp_path, "wb") as buffer:
            buffer.write(await file.read())
            
        # Parse text using the appropriate extractor
        raw_text = parse_cv(temp_path)
        
        if raw_text.startswith("Error"):
            raise HTTPException(status_code=400, detail=raw_text)
            
        # Extract metadata
        skills = extract_skills(raw_text)
        experience = extract_experience_years(raw_text)
        education = extract_education(raw_text)
        
        # Calculate AI Score
        # If job_requirements aren't passed, we assume 75.0 baseline.
        score = calculate_score(skills, raw_text, job_requirements or "")
        
        # Summary Generator
        summary = f"Candidate has {experience} years of experience and holds a {education}. Extracted {len(skills)} relevant technical skills."
        
        return {
            "extracted_skills": list(skills),
            "experience_years": experience,
            "education_level": education,
            "score": score,
            "summary": summary
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        # Cleanup
        if temp_path and os.path.exists(temp_path):
            try:
                os.remove(temp_path)
            except:
                pass
