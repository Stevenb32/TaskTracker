# Local Commands

## Start Backend

### Env: Dev

```powershell
dotnet run --project TaskTracker.Api --launch-profile http
```

#### Run Dev DB

```powershell
docker compose up -d
```

#### Add Dev Migration

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef migrations add MigrationName --project TaskTracker.Api --startup-project TaskTracker.Api
```

#### Update Dev DB

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

---

### Env: E2E

```powershell
dotnet run --project TaskTracker.Api --launch-profile e2e
```

#### Run E2E DB

```powershell
docker compose -f docker-compose.e2e.yml up -d
```

#### Add E2E Migration

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef migrations add MigrationName --project TaskTracker.Api --startup-project TaskTracker.Api
```

#### Update E2E DB

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

---

## Start Frontend

```powershell
cd .\TaskTracker.Ui\
npm install
npm run dev
```

---

## Run Playwright Tests in E2E

```powershell
cd .\TaskTracker.E2E.Tests\
npm install
npx playwright test
npx playwright test --ui
```
