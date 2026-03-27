import spacy
from typing import List, Tuple
import re

# Load English tokenizer, tagger, parser, NER and word vectors
try:
    nlp = spacy.load("en_core_web_sm")
except OSError:
    # Fallback if not downloaded
    import spacy.cli
    spacy.cli.download("en_core_web_sm")
    nlp = spacy.load("en_core_web_sm")

from spacy.matcher import PhraseMatcher

matcher = PhraseMatcher(nlp.vocab, attr="LOWER")

# Comprehensive modern tech skills library
TECH_SKILLS = [
    "python", "java", "c#", "c++", "javascript", "typescript", "ruby", "go", "php", "swift", "kotlin",
    "react", "angular", "vue", "next.js", "node.js", "express", "django", "flask", "fastapi", "spring boot",
    ".net core", "asp.net", "entity framework", "laravel",
    "sql", "mysql", "postgresql", "mongodb", "redis", "elasticsearch", "cassandra", "oracle", "sql server",
    "aws", "azure", "gcp", "google cloud", "docker", "kubernetes", "terraform", "ansible", "jenkins", "gitlab ci", 
    "machine learning", "deep learning", "nlp", "computer vision", "tensorflow", "pytorch", "scikit-learn", "pandas", "numpy",
    "agile", "scrum", "kanban", "figma", "adobe xd", "photoshop", "jira", "git", "github"
]

patterns = [nlp.make_doc(text) for text in TECH_SKILLS]
matcher.add("TechSkills", patterns)

def extract_skills(text: str) -> List[str]:
    """Uses spaCy PhraseMatcher to reliably extract skills from unstructured text."""
    doc = nlp(text)
    matches = matcher(doc)
    
    found_skills = set()
    for match_id, start, end in matches:
        span = doc[start:end]
        found_skills.add(span.text.lower())
        
    # Return formatted capitalized skills
    return [skill.title() if len(skill) > 3 else skill.upper() for skill in found_skills]

def extract_experience_years(text: str) -> int:
    """Rudimentary regex to find 'X years of experience'."""
    pattern = r'(\d+)\+?\s*(?:-\s*\d+\s*)?years?(?:\s+of)?\s+experience'
    matches = re.findall(pattern, text.lower())
    if matches:
        # Return max found if multiple
        return max([int(m) for m in matches])
    return 0
    
def extract_education(text: str) -> str:
    """Finds highest degree mentioned."""
    text_lower = text.lower()
    if 'phd' in text_lower or 'doctorate' in text_lower:
        return 'PhD'
    if 'master' in text_lower or 'm.sc' in text_lower or 'ma ' in text_lower:
        return 'Master'
    if 'bachelor' in text_lower or 'b.sc' in text_lower or 'ba ' in text_lower:
        return 'Bachelor'
    return 'Not Specified'
