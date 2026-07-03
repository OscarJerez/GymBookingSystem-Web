# Getting Started - Gym Booking System

This document walks you through building and running the complete system locally on Windows.

## Prerequisites

Ensure you have installed:
- **.NET 8 SDK** → https://dotnet.microsoft.com/download
- **Node.js 18+** → https://nodejs.org
- **Git** → https://git-scm.com (you already have this)

Verify installations:
```powershell
dotnet --version
node --version
npm --version
git --version
```

## Backend Setup (ASP.NET Core API)

Open **PowerShell** in the project root.

### 1. Navigate to API folder
```powershell
cd GymBookingSystem.API
```

### 2. Restore NuGet packages
```powershell
dotnet restore
```

### 3. Create database
```powershell
dotnet ef database update
```
This creates `gym_booking.db` with seed data (admin, owner, 3 sample classes).

### 4. Run the API
```powershell
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

**API is ready at**: http://localhost:5000
**Swagger docs at**: http://localhost:5000/swagger

The API will stay running. **Don't close this terminal.**

## Frontend Setup (React)

Open a **new PowerShell terminal** in the project root.

### 1. Navigate to frontend folder
```powershell
cd frontend
```

### 2. Install dependencies
```powershell
npm install
```

### 3. Start dev server
```powershell
npm run dev
```

Expected output:
```
  ➜  Local:   http://localhost:3000/
  ➜  press h to show help
```

**Frontend is ready at**: http://localhost:3000

## Test the System

Open your browser to **http://localhost:3000**

### Test Login (Owner)
1. Click **Login**
2. Username: `owner`
3. Password: `owner123`
4. Click **Login**

You should see:
- "owner (Owner)" in top-right
- 3 classes listed: Morning Yoga, HIIT Training, Spinning Class
- Book buttons on each class

### Test Booking
1. Click **Book Now** on any class
2. The booking appears in "My Bookings" sidebar
3. Status shows "Spots left: X" decreases
4. Click **Cancel Booking** to test cancellation

### Test Register (New Member)
1. Logout (top-right button)
2. Click **Register**
3. Fill in new username, email, password
4. Click **Register**
5. You're now logged in as a Member

### Test Admin (Optional)
1. Logout
2. Click **Login**
3. Username: `admin`
4. Password: `admin123`

Admin can:
- View all bookings (GET `/api/bookings/all`)
- Create/edit/delete classes (POST/PUT/DELETE `/api/classes`)

## API Testing (Postman/cURL)

### Register
```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!"
}
```

### Login
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "owner",
  "password": "owner123"
}
```

Response includes `token` — save it.

### Get All Classes
```bash
GET http://localhost:5000/api/classes
```

### Book a Class
```bash
POST http://localhost:5000/api/bookings
Authorization: Bearer {token_from_login}
Content-Type: application/json

{
  "classId": 1
}
```

### Get My Bookings
```bash
GET http://localhost:5000/api/bookings
Authorization: Bearer {token_from_login}
```

### Cancel Booking
```bash
DELETE http://localhost:5000/api/bookings/1
Authorization: Bearer {token_from_login}
```

## Troubleshooting

### Port 5000 already in use?
```powershell
# Find and kill process on port 5000
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Database issues?
```powershell
# Delete the database and recreate
rm gym_booking.db
dotnet ef database update
```

### Frontend won't connect to API?
- Make sure API is running on http://localhost:5000
- Check CORS is enabled in `Program.cs` for localhost:3000

### Clear browser cache?
- Press `F12` → Application → Clear Site Data

## Production Deployment

When ready for customers:

1. **Change JWT Secret** in `appsettings.json` to something long and random
2. **Use SQL Server** instead of SQLite:
   - Add `Microsoft.EntityFrameworkCore.SqlServer` NuGet package
   - Update connection string in `appsettings.json`
3. **Build React**: `npm run build` → uploads `dist/` folder to hosting
4. **Deploy API** to Azure, AWS, or on-premises server
5. **Set CORS origins** to your production domains

---

You're ready to demo to customers! Let me know if you hit any issues.
