# PixelAcademy

A production-ready .NET 8 backend API for the PixelAcademy e-learning platform, built with Clean Architecture, CQRS, MediatR, and Domain-Driven Design principles.

## Architecture

This solution follows **Clean Architecture** with four distinct layers:

| Layer | Project | Responsibility |
|-------|---------|--------------|
| Domain | `PixelAcademy.Domain` | Entities, enums, interfaces, domain logic, specifications |
| Application | `PixelAcademy.Application` | DTOs, commands, queries, validators, mappings, behaviors |
| Infrastructure | `PixelAcademy.Infrastructure` | Data access, repositories, caching, identity, external services |
| API | `PixelAcademy.API` | Controllers, middleware, DI configuration, Swagger |

## API Response Format

All successful responses are wrapped in a unified envelope:

```json
{
  "success": true,
  "data": { ... },
  "message": null,
  "errors": null,
  "timestamp": "2024-01-01T12:00:00Z",
  "traceId": "abc-123",
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 100,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

Error responses:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Validation failed",
  "errors": ["Email is required", "Password must be at least 6 characters"],
  "timestamp": "2024-01-01T12:00:00Z",
  "traceId": "abc-123"
}
```

## API Versioning

The API supports versioning via the `X-API-Version` header. Default version is `1.0`.

```bash
curl -H "X-API-Version: 1.0" http://localhost:5000/api/courses
```

## Rate Limiting

Production endpoints are rate-limited per IP:

| Endpoint | Limit | Period |
|----------|-------|--------|
| All endpoints | 100 | 1 minute |
| `POST /api/auth/login` | 5 | 1 minute |
| `POST /api/auth/register` | 3 | 5 minutes |

## Health Checks

- `GET /health` - Full health check (database + self)
- `GET /health/ready` - Liveness probe

## Technology Stack

- **.NET 8** (LTS)
- **PostgreSQL** (Npgsql.EntityFrameworkCore.PostgreSQL)
- **Redis** (distributed caching)
- **JWT Authentication** (AspNetCore.Authentication.JwtBearer)
- **Entity Framework Core** with migrations
- **MediatR** (CQRS implementation)
- **FluentValidation** (input validation)
- **AutoMapper** (DTO mapping)
- **Serilog** (structured logging)
- **Swagger / OpenAPI** with examples
- **API Versioning** (Asp.Versioning.Mvc)
- **Rate Limiting** (AspNetCoreRateLimit)
- **Health Checks**
- **Docker & Docker Compose** (dev + production)
- **xUnit + WebApplicationFactory** (integration testing)

## Completed Domains

- Authentication & Authorization (JWT + Refresh Tokens)
- Users & Roles (Student, Instructor, Admin)
- Courses & Lectures & Content Items
- Enrollments & Video Progress Tracking
- Media Assets
- Activation Codes & Wallet System
- Exams, Quizzes & Assignments
- Auto Grading & Manual Grading
- Notifications & Announcements
- Admin Dashboard & Analytics
- Audit Logging & Moderation
- Global Search with Pagination

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker & Docker Compose
- (Optional) PostgreSQL and Redis running locally

### Quick Start (Docker Development)

```bash
# Navigate to the repository
cd PixelAcademy

# Build and run the full stack (API + PostgreSQL + Redis)
docker compose -f docker/docker-compose.yml up --build
```

The API will be available at `http://localhost:5000` and Swagger UI at `http://localhost:5000/swagger`.

### Production Deployment

```bash
# Copy environment template
cp .env.example .env
# Edit .env with your production values

# Deploy with production compose (includes nginx reverse proxy)
docker compose -f docker/docker-compose.prod.yml up --build -d
```

### Local Development

```bash
# Start infrastructure services (PostgreSQL + Redis)
docker compose -f docker/docker-compose.dev.yml up -d

# Restore and build
dotnet restore
dotnet build

# Run the API
cd src/PixelAcademy.API
dotnet run
```

### Run Tests

```bash
# Run all integration tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal
```

### Scripts

| Script | Purpose |
|--------|---------|
| `scripts/build.sh` / `build.ps1` | Restore and build the solution |
| `scripts/run.sh` / `run.ps1` | Start dev infrastructure and run the API |
| `scripts/start-docker.sh` / `start-docker.ps1` | Build and run full Docker stack |

### Default Test Users (Seeded)

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@pixelacademy.com` | `Admin123!` |
| Instructor | `instructor@pixelacademy.com` | `Instructor123!` |
| Student | `student@pixelacademy.com` | `Student123!` |

### API Base URLs

| Environment | Base URL |
|-------------|----------|
| Local (Kestrel) | `http://localhost:5000` |
| Local HTTPS | `https://localhost:5001` |
| Docker | `http://localhost:5000` |
| Swagger UI | `http://localhost:5000/swagger` |

## API Endpoints

### Auth
- `POST /api/auth/register` - Register new account
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout
- `POST /api/auth/revoke-all` - Revoke all sessions
- `GET /api/auth/me` - Get current user profile

### Users
- `GET /api/users` - List all users (Admin)
- `GET /api/users/profile` - Get my profile
- `PUT /api/users/profile` - Update my profile
- `PUT /api/users/change-password` - Change password

### Courses
- `GET /api/courses` - List all courses
- `GET /api/courses/published` - List published courses
- `GET /api/courses/my-courses` - My courses (Instructor)
- `GET /api/courses/{id}` - Course details
- `POST /api/courses` - Create course (Instructor/Admin)
- `PUT /api/courses/{id}` - Update course (Owner)
- `DELETE /api/courses/{id}` - Soft delete course (Owner)

### Lectures
- `GET /api/courses/{courseId}/lectures` - List lectures
- `GET /api/courses/{courseId}/lectures/{id}` - Lecture details
- `POST /api/courses/{courseId}/lectures` - Create lecture (Owner)
- `PUT /api/courses/{courseId}/lectures/{id}` - Update lecture (Owner)
- `DELETE /api/courses/{courseId}/lectures/{id}` - Soft delete lecture (Owner)

### Content Items
- `GET /api/courses/{courseId}/lectures/{lectureId}/content-items` - List content items
- `POST /api/courses/{courseId}/lectures/{lectureId}/content-items` - Create content item (Owner)
- `PUT /api/courses/{courseId}/lectures/{lectureId}/content-items/{id}` - Update content item (Owner)
- `DELETE /api/courses/{courseId}/lectures/{lectureId}/content-items/{id}` - Soft delete content item (Owner)

### Enrollments
- `GET /api/enrollments` - My enrollments
- `GET /api/enrollments/{id}` - Enrollment details
- `POST /api/enrollments` - Enroll in course
- `PATCH /api/enrollments/{id}/status` - Update status (Admin)
- `POST /api/enrollments/{id}/rate` - Rate & review course

### Progress
- `GET /api/progress/course/{courseId}` - Course progress
- `POST /api/progress` - Update video progress

### Watch Sessions
- `POST /api/watchsessions/start` - Start watching lecture
- `POST /api/watchsessions/update-progress` - Update watch progress
- `POST /api/watchsessions/finish` - Finish watching
- `POST /api/watchsessions/signed-url/{lectureId}` - Get signed video URL
- `GET /api/watchsessions/continue` - Continue watching list
- `GET /api/watchsessions/completed` - Completed lectures
- `GET /api/watchsessions/course-progress/{courseId}` - Course progress percentage

### Analytics (Instructor/Admin)
- `GET /api/analytics/lecture/{lectureId}` - Lecture analytics
- `GET /api/analytics/course/{courseId}` - Course analytics

### Media
- `GET /api/media/course/{courseId}` - Course media
- `GET /api/media/lecture/{lectureId}` - Lecture media
- `POST /api/media` - Upload media (URL-based)
- `POST /api/media/upload` - Upload file (multipart/form-data, max 100MB)
  - Supported: video/mp4, video/webm, video/ogg, audio/mpeg, audio/wav, application/pdf, image/jpeg, image/png, image/gif
- `DELETE /api/media/{id}` - Delete media

### Exams
- `GET /api/exams` - List exams
- `GET /api/exams/{id}` - Exam details with questions
- `POST /api/exams` - Create exam (Instructor/Admin)
- `PUT /api/exams/{id}` - Update exam (Owner)
- `DELETE /api/exams/{id}` - Delete exam (Owner)
- `POST /api/exams/{id}/publish` - Publish/unpublish exam (Owner)
- `GET /api/exams/{examId}/questions` - List questions
- `POST /api/exams/{examId}/questions` - Create question (Owner)
- `PUT /api/exams/questions/{questionId}` - Update question
- `DELETE /api/exams/questions/{questionId}` - Delete question
- `POST /api/exams/{examId}/start` - Start exam attempt (Student)
- `GET /api/exams/attempts/{attemptId}` - Get attempt details
- `POST /api/exams/attempts/{attemptId}/submit` - Submit exam attempt (Student)
- `GET /api/exams/my-results` - My exam results (Student)
- `GET /api/exams/{examId}/analytics` - Exam analytics (Instructor/Admin)

### Assignments
- `GET /api/assignments` - List assignments
- `GET /api/assignments/{id}` - Assignment details
- `POST /api/assignments` - Create assignment (Instructor/Admin)
- `PUT /api/assignments/{id}` - Update assignment (Owner)
- `DELETE /api/assignments/{id}` - Delete assignment (Owner)
- `POST /api/assignments/{id}/publish` - Publish/unpublish assignment (Owner)
- `POST /api/assignments/submit` - Submit assignment (Student)
- `GET /api/assignments/{assignmentId}/submissions` - List submissions (Instructor/Admin)
- `GET /api/assignments/{assignmentId}/my-submission` - My submission (Student)
- `POST /api/assignments/grade` - Grade submission (Instructor/Admin)

### Activation Codes
- `POST /api/activationcodes/generate` - Generate activation code (Admin)
- `POST /api/activationcodes/redeem` - Redeem activation code (Student)
- `POST /api/activationcodes/{id}/disable` - Disable code (Admin)
- `GET /api/activationcodes` - List codes
- `GET /api/activationcodes/{id}` - Code details
- `GET /api/activationcodes/validate/{code}` - Validate code

### Wallet
- `GET /api/wallet/balance` - Get my wallet balance (Student)
- `GET /api/wallet/transactions` - Get my wallet transactions (Student)

### Notifications
- `GET /api/notifications` - My notifications
- `GET /api/notifications/unread` - My unread notifications
- `GET /api/notifications/unread-count` - Unread notification count
- `GET /api/notifications/{id}` - Notification details
- `POST /api/notifications` - Create notification (Admin)
- `POST /api/notifications/{id}/mark-read` - Mark as read
- `POST /api/notifications/mark-all-read` - Mark all as read
- `DELETE /api/notifications/{id}` - Delete notification

### Announcements
- `GET /api/announcements` - List published announcements
- `GET /api/announcements/{id}` - Announcement details
- `GET /api/announcements/course/{courseId}` - Course announcements
- `POST /api/announcements` - Create announcement (Instructor/Admin)
- `PUT /api/announcements/{id}` - Update announcement (Owner)
- `DELETE /api/announcements/{id}` - Delete announcement (Owner)
- `POST /api/announcements/{id}/publish` - Publish announcement

### Admin
- `GET /api/admin/dashboard` - Admin dashboard analytics
- `POST /api/admin/users/{userId}/ban` - Ban user
- `POST /api/admin/users/{userId}/unban` - Unban user
- `POST /api/admin/courses/{courseId}/disable` - Disable course
- `POST /api/admin/courses/{courseId}/enable` - Enable course
- `POST /api/admin/lectures/{lectureId}/disable` - Disable lecture
- `POST /api/admin/lectures/{lectureId}/enable` - Enable lecture

### Audit Logs
- `GET /api/auditlogs` - List audit logs (Admin)
- `GET /api/auditlogs/admin-actions` - Admin action logs (Admin)

### Search
- `GET /api/search?q={query}` - Global search (courses, lectures, exams, students)
- `GET /api/search/courses?q={query}&category={cat}&level={level}` - Filtered course search

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=pixelacademy;Username=postgres;Password=postgres",
    "RedisConnection": "localhost:6379"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "PixelAcademy",
    "Audience": "PixelAcademyClient",
    "ExpirationMinutes": "60"
  }
}
```

### Environment Variables (Docker)

- `ASPNETCORE_ENVIRONMENT` - `Development`, `Production`, or `Docker`
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `ConnectionStrings__RedisConnection` - Redis connection string

## Database Migrations

Migrations are applied automatically on startup. To create a new migration:

```bash
cd src/PixelAcademy.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../PixelAcademy.API
```

## Project Structure

```
PixelAcademy/
├── PixelAcademy.sln
├── .env.example
├── docker/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── docker-compose.dev.yml
│   ├── docker-compose.prod.yml
│   └── nginx.conf
├── scripts/
│   ├── build.sh / build.ps1
│   ├── run.sh / run.ps1
│   └── start-docker.sh / start-docker.ps1
├── tests/
│   └── PixelAcademy.API.Tests/
│       ├── CustomWebApplicationFactory.cs
│       ├── ApiResponseExtensions.cs
│       └── *.cs (integration tests)
└── src/
    ├── PixelAcademy.Domain/
    │   ├── Common/
    │   ├── Entities/
    │   ├── Enums/
    │   ├── Exceptions/
    │   ├── Interfaces/
    │   ├── Services/
    │   └── Specifications/
    ├── PixelAcademy.Application/
    │   ├── Abstractions/
    │   ├── Behaviors/
    │   ├── Commands/
    │   ├── DTOs/
    │   ├── Exceptions/
    │   ├── Interfaces/
    │   ├── Mappings/
    │   ├── Queries/
    │   └── Validators/
    ├── PixelAcademy.Infrastructure/
    │   ├── Cache/
    │   ├── Data/
    │   │   ├── Configurations/
    │   │   ├── Seed/
    │   │   └── Migrations/
    │   ├── Identity/
    │   ├── Repositories/
    │   ├── Services/
    │   ├── UnitOfWork/
    │   └── Extensions/
    └── PixelAcademy.API/
        ├── Controllers/
        ├── Extensions/
        ├── Filters/
        ├── Middleware/
        ├── Models/
        └── appsettings.*.json
```

## Testing

The solution includes **129 integration tests** covering all major domains:

- Phase 1: Auth (register, login, refresh, logout, me, 401) - 8 tests
- Phase 2: Courses, Lectures, Content Items, Enrollments - 24 tests
- Phase 3: Activation Codes, Wallet, Transactions - 21 tests
- Phase 4: Watch Sessions, Video Progress, Analytics - 17 tests
- Phase 5: Exams, Questions, Attempts, Assignments, Grading - 26 tests
- Phase 6: Notifications, Announcements, Admin, Audit Logs, Search - 33 tests

Run all tests:
```bash
dotnet test
```

## License

MIT
