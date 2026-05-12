# Getting Started

## Prerequisites

- .NET SDK
- Node.js / npm
- Docker Desktop
- PostgreSQL extension for VS Code, optional
- Playwright browsers

## Clone the repo
git clone <repo-url>
cd TaskTracker

## Restore backend dependencies
dotnet restore

## Install frontend dependencies
cd TaskTracker.Ui
npm install

## Install Playwright dependencies
cd ../TaskTracker.E2E.Tests
npm install
npx playwright install