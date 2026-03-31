# Starts API + Vite in two separate PowerShell windows (classic local dev).
# Run from repo root:  .\scripts\start-local.ps1

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot
$ApiDir = Join-Path $Root "backend\HRSystem.API"
$UiDir = Join-Path $Root "frontend"

if (-not (Test-Path $ApiDir)) { throw "API folder not found: $ApiDir" }
if (-not (Test-Path $UiDir)) { throw "Frontend folder not found: $UiDir" }

Write-Host "Opening window 1: dotnet API -> http://localhost:5254"
Start-Process powershell -WorkingDirectory $ApiDir -ArgumentList @(
    "-NoExit", "-Command",
    "Write-Host 'HR API (Ctrl+C to stop)'; dotnet run --urls `"http://localhost:5254`""
)

Write-Host "Opening window 2: Vite -> http://localhost:3000"
Start-Process powershell -WorkingDirectory $UiDir -ArgumentList @(
    "-NoExit", "-Command",
    "if (-not (Test-Path node_modules)) { npm install }; Write-Host 'Vite (Ctrl+C to stop)'; npm run dev"
)

Write-Host ""
Write-Host "Done. Swagger: http://localhost:5254/swagger"
Write-Host "Login: admin@hr.com / admin123"
