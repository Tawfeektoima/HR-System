from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.core.config import settings
from app.routers import cv_router, matching_router

app = FastAPI(
    title=settings.PROJECT_NAME,
    openapi_url=f"{settings.API_V1_STR}/openapi.json"
)

# Set all CORS enabled origins
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.ALLOWED_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(cv_router.router, tags=["cv_analysis"])
app.include_router(matching_router.router, tags=["matching"])

@app.get("/")
def read_root():
    return {"message": "Welcome to HR AI Service API"}

@app.get("/health")
def health_check():
    return {"status": "ok"}
