# AI-Powered HR Recruitment System

A comprehensive platform to manage and streamline the hiring process.

## Overview

This system provides a centralized platform to handle the entire recruitment lifecycle in one place, replacing scattered Excel sheets and manual processes.

## Tech Stack

| Component | Technology |
|-----------|------------|
| Backend | ASP.NET Core 8 |
| AI Service | Python FastAPI |
| Database | SQL Server / PostgreSQL |
| Frontend | React.js |

## Features

- Job application portal for candidates
- CV upload and AI-powered parsing
- Candidate scoring and auto-ranking
- HR Dashboard with search and filters
- Applicant Tracking System (ATS)
- Recruitment analytics dashboard

## ATS Stages

Applied -> Phone Interview -> Technical Interview -> Final Interview -> Accepted/Rejected

## Project Structure

```
HR-System/
|-- backend/          # ASP.NET Core API
|-- ai-service/       # Python FastAPI AI
|-- frontend/         # React Web UI
|-- database/         # SQL Scripts
`-- docker-compose.yml
```

## Future Enhancements

- WhatsApp Bot integration
- Automated chatbot interviews
- Voice analysis for language evaluation
- Auto interview scheduling
