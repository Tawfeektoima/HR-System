from typing import List
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity

def calculate_score(candidate_skills: List[str], candidate_text: str, job_requirements: str) -> float:
    """
    Calculates candidate fitness score (0-100) using:
    1. Direct Skill Matching (weighted heavily).
    2. TF-IDF Cosine Similarity of full CV text against job description (for context matching).
    """
    if not job_requirements or not candidate_text:
        return 0.0

    # Clean empty strings
    candidate_skills = [s.lower() for s in candidate_skills]
    
    # 1. Direct Skill Matching (using simple keywords extraction from job_requirements)
    # We'll just tokenize the job requirements roughly for direct hits
    job_req_words = set(job_requirements.lower().replace(',', ' ').replace('.', ' ').split())
    
    matched_skills = 0
    total_requested_skills = 0
    
    # If a known skill from the candidate CV is explicitly in the JD
    for skill in candidate_skills:
        skill_parts = skill.split()
        # if the skill is directly mentioned in JD
        if all(part in job_req_words for part in skill_parts):
            matched_skills += 1

    # Assuming a baseline of 5 core skills usually asked in a JD if we can't extract them perfectly
    # To be extremely accurate, we should run skill_extractor on the job_requirements too!
    from app.services.skill_extractor import extract_skills
    job_req_skills = extract_skills(job_requirements)
    
    direct_match_score = 0.0
    if job_req_skills:
        direct_matches = set(candidate_skills).intersection(set([s.lower() for s in job_req_skills]))
        direct_match_score = (len(direct_matches) / len(job_req_skills)) * 100
    else:
        # If the JD has no recognizable skills, we rely entirely on semantic similarity
        direct_match_score = 50.0 

    # 2. Semantic Similarity via TF-IDF
    vectorizer = TfidfVectorizer(stop_words='english')
    tfidf_matrix = vectorizer.fit_transform([job_requirements, candidate_text])
    
    # matrix[0] is JD, matrix[1] is CV
    cosine_sim = cosine_similarity(tfidf_matrix[0:1], tfidf_matrix[1:2])[0][0]
    semantic_score = float(cosine_sim) * 100

    # Final Score: 60% Direct Skills, 40% Semantic Content Match
    final_score = (direct_match_score * 0.6) + (semantic_score * 0.4)
    
    # Cap at 100
    return min(round(final_score, 2), 100.0)
