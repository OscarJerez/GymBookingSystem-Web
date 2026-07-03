# Gym Booking System - Full Stack Web Application

A modern, production-ready gym booking platform built with:
- **Backend**: ASP.NET Core 8 REST API with JWT authentication
- **Frontend**: React 18 with TypeScript and Tailwind CSS
- **Database**: SQLite with Entity Framework Core
- **Auth**: JWT Bearer tokens with role-based access control

## Quick Start

### Backend Setup (Windows)

```powershell
cd GymBookingSystem.API
dotnet build
dotnet ef database update
dotnet run
```

The API runs on `http://localhost:5000` and includes Swagger UI at `/swagger`.

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

The frontend runs on `http://localhost:3000`.

## Default Accounts

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Admin |
| owner | owner123 | Owner |

New members can register on the frontend.

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new member
- `POST /api/auth/login` - Login and get JWT token

### Classes
- `GET /api/classes` - List all active classes
- `GET /api/classes/{id}` - Get class details
- `POST /api/classes` - Create class (Owner/Admin only)
- `PUT /api/classes/{id}` - Update class (Owner/Admin only)
- `DELETE /api/classes/{id}` - Soft delete class (Owner/Admin only)

### Bookings
- `GET /api/bookings` - Get my bookings (Members)
- `GET /api/bookings/all` - Get all bookings (Admin only)
- `POST /api/bookings` - Book a class
- `DELETE /api/bookings/{id}` - Cancel booking

## Features

✅ User registration and login with JWT auth
✅ Role-based access control (Member, Owner, Admin)
✅ Class listing with real-time availability
✅ Booking and cancellation
✅ Admin dashboard (ready to build)
✅ Responsive UI with Tailwind CSS
✅ SQLite database with EF Core migrations
✅ Swagger API documentation

## Database Schema

### Users
- Id, Username, Email, PasswordHash, Role, CreatedAt, IsActive

### GymClasses
- Id, Name, Description, StartTime, EndTime, Capacity, InstructorName, CreatedAt, IsActive

### Bookings
- Id, UserId, ClassId, BookedAt, Status (Active/Cancelled/Completed)

## Development

Make sure you have:
- .NET 8 SDK installed
- Node.js 18+ installed
- CORS is enabled for localhost:3000 and localhost:3001

## Production Deployment

1. Change JWT secret in `appsettings.json`
2. Use SQL Server or PostgreSQL instead of SQLite
3. Set up HTTPS and proper CORS origins
4. Run `dotnet ef database update` on the target database
5. Build React: `npm run build` → deploy `dist/` folder

---

Built during InFiNetCode Bootcamp 2026. Ready for customers.
