import os
from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    PROJECT_NAME: str = "HR AI Service"
    API_V1_STR: str = "/api/v1"
    SECRET_KEY: str = os.getenv("SECRET_KEY", "dev_secret_key_123")
    ALLOWED_ORIGINS: list[str] = ["http://localhost:5000", "http://localhost:5001"]

    class Config:
        case_sensitive = True

settings = Settings()
