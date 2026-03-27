import spacy

def download_models():
    print("Downloading spaCy models...")
    spacy.cli.download("en_core_web_sm")
    print("Models downloaded successfully!")

if __name__ == "__main__":
    download_models()
