# TaskPulse.Api — Ready-to-run API

This package is a cleaned, consolidated version of the TaskPulse backend. It keeps the migration folder at the project root (`Migrations`) and removes the duplicate/ambiguous repository and service definitions that caused the compile errors.

## Requirements
- .NET 10 SDK
- SQL Server LocalDB (the default connection string targets `(localdb)\MSSQLLocalDB`)
- Visual Studio 2026 or another .NET 10 compatible IDE

## Run
1. Extract the ZIP.
2. Open `TaskPulse.Api.csproj`.
3. Restore/build the project.
4. Run the project.
5. Swagger opens in Development.
6. The app automatically applies the existing `InitialCreate` migration and seeds an admin user on an empty database.

### Seed admin
- Email: `admin@taskpulse.local`
- Password: `Admin@12345`

Change these values in `appsettings.json` before using the app outside local development.

## Main API areas
- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `GET/PUT/DELETE /api/Users/...`
- `GET/POST/PUT/DELETE /api/Projects/...`
- `GET/POST/PUT/DELETE /api/projects/{projectId}/members/...`
- `GET/POST/PUT/DELETE /api/projects/{projectId}/tasks/...`
- `GET /api/projects/{projectId}/activity-logs`

## Important cleanup
There should be exactly one definition of each of these:
- `IProjectRepository` in `Repositories/Interfaces`
- `ProjectRepository` in `Repositories/Projects`
- `IProjectMemberRepository` in `Repositories/Interfaces`
- `ProjectMemberRepository` in `Repositories/ProjectMembers`
- `IProjectMemberService` in `Services/ProjectMembers`
- `ProjectMemberService` in `Services/ProjectMembers`

Do not add another interface/class with the same name under a second namespace.

## Database
The migration is intentionally kept in the project-root `Migrations` folder, matching the structure already used by the project.
