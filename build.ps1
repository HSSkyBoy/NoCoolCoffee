# Build script for NoCoolCoffee (Cinnamon Coffee)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Building Cinnamon Coffee (.dll)       " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Ensure output directory exists
$outputDir = Join-Path $PSScriptRoot "Release"
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

if (Get-Command "dotnet" -ErrorAction SilentlyContinue) {
    Write-Host "Found dotnet SDK. Building project..." -ForegroundColor Green
    dotnet build NoCoolCoffee.csproj -c Release
    if ($LASTEXITCODE -eq 0) {
        Copy-Item -Path "$PSScriptRoot\bin\Release\CinnamonCoffee.dll" -Destination $outputDir -Force
        Copy-Item -Path "$PSScriptRoot\CinnamonCoffee.cfg" -Destination $outputDir -Force
        Copy-Item -Path "$PSScriptRoot\CinnamonCoffeeALife.ini" -Destination $outputDir -Force
        Write-Host "Build Succeeded! Files packaged to: $outputDir" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "dotnet build failed!" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "dotnet SDK is not installed on local host." -ForegroundColor Yellow
    Write-Host "Please use GitHub Actions Workflow or install .NET SDK 8.0+ to compile locally." -ForegroundColor Yellow
}
