import re
from typing import List

def calculate_score(candidate_skills: List[str], candidate_text: str, job_requirements: str) -> float:
    """
    Calculates candidate fitness score (0-100) using:
    1. Direct Skill Matching (weighted heavily).
    2. Simple Jaccard Similarity of keywords (for context matching).
    """
    if not job_requirements or not candidate_text:
        return 0.0

    # 1. Direct Skill Matching
    candidate_skills = [s.lower() for s in candidate_skills]
    from app.services.skill_extractor import extract_skills
    job_req_skills = [s.lower() for s in extract_skills(job_requirements)]
    
    direct_match_score = 0.0
    if job_req_skills:
        direct_matches = set(candidate_skills).intersection(set(job_req_skills))
        direct_match_score = (len(direct_matches) / len(job_req_skills)) * 100
    else:
        # If the JD has no recognizable skills, we rely entirely on keyword similarity
        direct_match_score = 40.0 

    # 2. Keyword Similarity (Simplified Semantic)
    def get_keywords(text):
        return set(re.findall(r'\w+', text.lower()))
    
    jd_keywords = get_keywords(job_requirements)
    cv_keywords = get_keywords(candidate_text)
    
    if not jd_keywords:
        return direct_match_score

    intersection = jd_keywords.intersection(cv_keywords)
    union = jd_keywords.union(cv_keywords)
    jaccard_sim = len(intersection) / len(union) if union else 0
    
    semantic_score = jaccard_sim * 100 * 5 # Scale up as Jaccard is usually low
    
    # Final Score: 70% Direct Skills, 30% Keyword Content Match
    final_score = (direct_match_score * 0.7) + (semantic_score * 0.3)
    
    # Cap at 100
    return min(round(final_score, 2), 100.0)
