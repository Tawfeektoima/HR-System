import re
from pdfminer.high_level import extract_text as extract_pdf_lines
from pdfminer.pdfparser import PDFSyntaxError

def extract(file_path: str) -> str:
    """Extracts raw text from a PDF file."""
    try:
        text = extract_pdf_lines(file_path)
        # Clean up excessive newlines and spaces
        text = re.sub(r'\n+', '\n', text)
        text = re.sub(r' +', ' ', text)
        return text.strip()
    except PDFSyntaxError:
        return "Error parsing PDF file."
    except Exception as e:
        return f"Error: {str(e)}"
