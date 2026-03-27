from fastapi import APIRouter
from pydantic import BaseModel
from typing import List

router = APIRouter()

class JobMatchRequest(BaseModel):
    candidate_skills: List[str]
    job_requirements: str

class MatchResponse(BaseModel):
    similarity_score: float
    missing_skills: List[str]

@router.post("/match-job", response_model=MatchResponse)
async def match_job(request: JobMatchRequest):
    # Mock response
    return {
        "similarity_score": 0.82,
        "missing_skills": ["AWS", "Docker"]
    }
