import os
from . import pdf_extractor, docx_extractor, ocr_service

def parse_cv(file_path: str) -> str:
    """Detects file type and delegates to the right extractor."""
    _, ext = os.path.splitext(file_path.lower())
    
    if ext == '.pdf':
        return pdf_extractor.extract(file_path)
    elif ext in ['.doc', '.docx']:
        return docx_extractor.extract(file_path)
    elif ext in ['.jpg', '.jpeg', '.png']:
        return ocr_service.extract(file_path)
    elif ext == '.txt':
        with open(file_path, 'r', encoding='utf-8') as f:
            return f.read()
    else:
        # Default fallback
        return "Unknown format. Extracted text goes here."
