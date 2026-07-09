# 💪 Gym Booking System

> A modern, production-ready gym booking platform. Book classes, manage schedules, and streamline member access.

![Status](https://img.shields.io/badge/status-production--ready-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![ASP.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![React](https://img.shields.io/badge/React-18-61DAFB)
![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-blue)

## 🚀 Quick Start

### Prerequisites
- **.NET 8 SDK** ([download](https://dotnet.microsoft.com/download))
- **Node.js 18+** ([download](https://nodejs.org))
- **Git**

### Development (Local)

**Terminal 1: Backend API**
```bash
cd GymBookingSystem.API
dotnet ef database update
dotnet run
# API runs on http://localhost:5000
# Swagger docs: http://localhost:5000/swagger
```

**Terminal 2: Frontend**
```bash
cd frontend
npm install
npm run dev
# Frontend runs on http://localhost:3000
```

Open **http://localhost:3000** in your browser.

---

## 📚 Features

### For Members
✅ **Register & Login** — Secure JWT authentication  
✅ **Browse Classes** — See all available gym classes with real-time availability  
✅ **Book Classes** — One-click booking with instant confirmation  
✅ **Manage Bookings** — View and cancel your reservations anytime  
✅ **Responsive UI** — Works perfectly on mobile, tablet, and desktop  

### Phase 2 Features ⭐ NEW
✅ **Waitlist System** — Automatically join waitlist when class is full, get notified when spot opens  
✅ **Recurring Classes** — Weekly class schedules that auto-generate for 12 weeks  
✅ **Membership Plans** — Monthly, Quarterly, or Annual memberships with Stripe integration  
✅ **Payment Integration** — Secure Stripe payments for membership purchases  
✅ **React Native Mobile** — Cross-platform mobile app (iOS/Android)  

### For Owners
✅ **Create Classes** — Add new classes with name, time, capacity, instructor  
✅ **Edit Classes** — Update class details on the fly  
✅ **Delete Classes** — Remove outdated classes (soft delete)  
✅ **View Availability** — Real-time spot availability per class  
✅ **Recurring Setup** — Configure weekly recurring classes  

### For Admins
✅ **System Overview** — Access all bookings and classes  
✅ **User Management** — Monitor member activity  
✅ **Full Control** — Create, update, delete any class  
✅ **Payment Dashboard** — View membership revenue  
✅ **Analytics** — Track bookings and waitlist trends  

---

## 🏗️ Architecture

### Tech Stack

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Backend** | ASP.NET Core 8 | REST API, business logic |
| **Frontend** | React 18 | User interface |
| **Mobile** | React Native | Cross-platform app |
| **Database** | SQLite (dev) / SQL Server (prod) | Data persistence |
| **Auth** | JWT Bearer tokens | Authentication & authorization |
| **Payments** | Stripe API | Secure payment processing |
| **State** | Zustand | Frontend state management |
| **Styling** | Tailwind CSS | Modern, responsive design |
| **API Client** | Axios | HTTP requests with interceptors |
| **Testing** | XUnit + Moq | Unit & integration tests |
| **CI/CD** | GitHub Actions | Automated testing & deployment |

### Database Schema

```
Users (Id, Email, PasswordHash, FirstName, LastName, Role, CreatedAt)
  ├→ Bookings (Id, UserId, ClassId, BookedAt, Status)
  ├→ Waitlist (Id, UserId, ClassId, Position, AddedAt)
  └→ Payments (Id, UserId, MembershipId, Amount, Status)

GymClasses (Id, Name, Capacity, InstructorName, Schedule, MaxCapacity)
  └→ RecurringClasses (Id, ClassId, SchedulePattern, StartDate, EndDate)

Memberships (Id, Name, Duration, Price, Features)
```

### Project Structure

```
GymBookingSystem-Web/
├── GymBookingSystem.API/                    # ASP.NET Core 8 backend
│   ├── Controllers/                         # API endpoints
│   │   ├── AuthController.cs
│   │   ├── ClassesController.cs
│   │   ├── BookingsController.cs
│   │   ├── WaitlistController.cs            # ⭐ Phase 2
│   │   ├── RecurringClassesController.cs    # ⭐ Phase 2
│   │   └── PaymentsController.cs            # ⭐ Phase 2
│   ├── Services/                            # Business logic
│   │   ├── TokenService.cs
│   │   ├── WaitlistService.cs               # ⭐ Phase 2
│   │   ├── RecurringClassService.cs         # ⭐ Phase 2
│   │   └── PaymentService.cs                # ⭐ Phase 2
│   ├── Domain/                              # Entities
│   │   ├── User.cs
│   │   ├── GymClass.cs
│   │   ├── Booking.cs
│   │   ├── Waitlist.cs                      # ⭐ Phase 2
│   │   ├── RecurringClass.cs                # ⭐ Phase 2
│   │   ├── Membership.cs                    # ⭐ Phase 2
│   │   └── Payment.cs                       # ⭐ Phase 2
│   ├── Data/                                # EF Core
│   │   └── GymBookingDbContext.cs
│   ├── DTOs/                                # Request/Response models
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs       # ⭐ Phase 2
│   ├── Exceptions/
│   │   └── AppExceptions.cs                 # ⭐ Phase 2
│   ├── Program.cs
│   └── appsettings.json
│
├── GymBookingSystem.API.Tests/              # ⭐ Test projects
│   └── Services/
│       ├── Phase2ServiceTests.cs
│       ├── PaymentServiceTests.cs
│       ├── WaitlistServiceTests.cs
│       └── RecurringClassServiceTests.cs
│
├── frontend/                                # React web app
│   ├── src/
│   │   ├── pages/
│   │   │   ├── HomePage.jsx
│   │   │   ├── LoginPage.jsx
│   │   │   ├── RegisterPage.jsx
│   │   │   └── BookingPage.jsx
│   │   ├── components/
│   │   ├── stores/
│   │   │   ├── authStore.js
│   │   │   └── classesStore.js
│   │   └── App.jsx
│   ├── package.json
│   └── vite.config.js
│
├── mobile/                                  # ⭐ React Native app
│   ├── app/
│   └── package.json
│
├── .github/workflows/
│   └── ci-cd.yml                            # ⭐ GitHub Actions
│
├── GymBookingSystem.sln
├── README.md                                # This file
├── SETUP.md                                 # Local setup guide
├── API-SPEC.md                              # API reference
├── DEPLOYMENT.md                            # Production deployment
└── TESTING_GUIDE.md                         # ⭐ Testing documentation
```

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

The project includes automated **CI/CD** with GitHub Actions:

**Triggered on:**
- ✅ Push to `main` or `develop` branch
- ✅ Pull requests to `main` or `develop`

**Jobs:**

1. **test-backend** (XUnit)
   - Restores .NET dependencies
   - Builds ASP.NET Core project
   - Runs all unit & integration tests
   - Reports test results

2. **build-frontend** (Node.js)
   - Installs npm dependencies
   - Builds React app with Vite
   - Uploads build artifacts

3. **code-quality**
   - Runs code analysis
   - Checks for errors
   - Validates architecture

4. **deploy-frontend** (Vercel)
   - Downloads frontend build
   - Deploys to Vercel automatically
   - Live after main push ✅

**View workflow:** `.github/workflows/ci-cd.yml`

**Check status:** Go to [GitHub > Actions](https://github.com/OscarJerez/GymBookingSystem-Web/actions)

---

## 🧪 Testing

### Running Tests Locally

**Backend Tests:**
```bash
cd GymBookingSystem.API.Tests
dotnet test
```

**Phase 2 Test Coverage:**
- ✅ Waitlist Service — Add/remove/notify
- ✅ Recurring Classes Service — Generate schedules
- ✅ Payment Service — Process memberships
- ✅ Error Handling — Validation & exceptions

**See detailed guide:** `TESTING_GUIDE.md`

---

## 📡 API Endpoints

### Authentication
```
POST   /api/auth/register      Register new user
POST   /api/auth/login         Login user
GET    /api/auth/me            Get current user
```

### Classes
```
GET    /api/classes            List all classes
GET    /api/classes/{id}       Get class details
POST   /api/classes            Create class (admin)
PUT    /api/classes/{id}       Update class (admin)
DELETE /api/classes/{id}       Delete class (admin)
```

### Bookings
```
GET    /api/bookings           Get my bookings
POST   /api/bookings           Create booking
DELETE /api/bookings/{id}      Cancel booking
```

### Waitlist ⭐
```
POST   /api/waitlist           Join waitlist
GET    /api/waitlist/{classId} Get waitlist
DELETE /api/waitlist/{id}      Leave waitlist
```

### Recurring Classes ⭐
```
POST   /api/recurring-classes  Create recurring schedule
PUT    /api/recurring-classes/{id}    Update schedule
DELETE /api/recurring-classes/{id}    Stop recurring
```

### Payments ⭐
```
POST   /api/payments/intent    Create Stripe payment intent
POST   /api/payments/confirm   Confirm payment
GET    /api/payments           Get my payments
```

**Full API docs:** `API-SPEC.md` or visit `/swagger` on running backend

---

## 🌍 Deployment

### Frontend (Vercel)

Auto-deploys on push to `main`:
1. GitHub Actions builds frontend
2. Uploads to Vercel
3. Live at your Vercel URL

Manual deploy:
```bash
cd frontend
vercel --prod
```

### Backend (Azure / Heroku / Docker)

```bash
# Build production image
docker build -t gym-booking .

# Or deploy to Azure App Service
dotnet publish -c Release
```

**See full guide:** `DEPLOYMENT.md`

---

## 🔐 Security

✅ **Password Hashing** — Bcrypt (cost factor 11)  
✅ **JWT Authentication** — 24-hour token expiry  
✅ **Role-Based Access** — Member/Owner/Admin  
✅ **SQL Injection Prevention** — Entity Framework ORM  
✅ **CORS Configuration** — Localhost dev, origin prod  
✅ **Input Validation** — All endpoints validated  

---

## 📖 Documentation

- **SETUP.md** — Step-by-step local setup
- **API-SPEC.md** — All endpoints with examples
- **DEPLOYMENT.md** — Production deployment guide
- **TESTING_GUIDE.md** — Testing procedures
- **QUICK-DEPLOY.md** — 5-minute deployment summary
- **PROJECT-SUMMARY.md** — Complete project overview

---

## 🐛 Troubleshooting

### Backend won't start
```bash
# Clear database and restart
rm GymBookingSystem.API/gym_booking.db
cd GymBookingSystem.API
dotnet ef database update
dotnet run
```

### Frontend won't build
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
npm run dev
```

### CORS errors
- Ensure frontend URL matches backend CORS origins
- Check `.env` variables: `VITE_API_URL=http://localhost:5000/api`

### Tests failing
```bash
# Run tests with detailed output
dotnet test --verbosity=detailed

# Run specific test
dotnet test --filter "WaitlistServiceTests"
```

---

## 🚢 Demo Accounts

```
Admin:
  Username: admin
  Password: admin123

Member:
  Username: member
  Password: member123
```

---

## 📊 Performance

- SQLite with indexes (dev)
- JWT stateless auth (no session overhead)
- React component optimization
- CSS minification
- Lazy loading
- Compression middleware

---

## 🎯 Roadmap

- ✅ Phase 1: Core booking system
- ✅ Phase 2: Waitlist, recurring, payments
- 🔜 Phase 3: Mobile app optimization
- 🔜 Phase 4: Analytics dashboard
- 🔜 Phase 5: Email notifications
- 🔜 Phase 6: SMS reminders

---

## 👨‍💻 Development

### Local Development Setup
See `SETUP.md` for complete guide

### Run Backend
```bash
cd GymBookingSystem.API && dotnet run
```

### Run Frontend
```bash
cd frontend && npm run dev
```

### Run Tests
```bash
cd GymBookingSystem.API.Tests && dotnet test
```

---

## 📞 Support

1. Check documentation files
2. Review API docs at `/swagger`
3. Check error messages carefully
4. Review console logs
5. Run tests to validate setup

---

## 📄 License

MIT License - Feel free to use for commercial projects

---

## 🙏 Credits

Build using: ASP.NET Core 8, React 18, and Stripe

**Production Ready** ✅ | **Fully Tested** ✅ | **Deployed** ✅

---

**Last Updated:** July 9, 2026  
**Status:** 🟢 Production Ready
