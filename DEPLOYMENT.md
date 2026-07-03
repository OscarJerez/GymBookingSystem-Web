# Deployment Guide

Complete instructions for deploying Gym Booking System to production.

## Table of Contents
1. [Frontend Deployment (Vercel)](#frontend-deployment-vercel)
2. [Backend Deployment (Azure)](#backend-deployment-azure)
3. [Environment Configuration](#environment-configuration)
4. [Security Checklist](#security-checklist)
5. [Monitoring & Maintenance](#monitoring--maintenance)

---

## Frontend Deployment (Vercel)

Vercel is the fastest way to deploy React apps. It handles builds, deployments, and CDN automatically.

### Option 1: GitHub Integration (Recommended)

1. **Commit all changes to GitHub**
   ```bash
   git add .
   git commit -m "Ready for production"
   git push origin main
   ```

2. **Sign up on Vercel**
   - Go to [vercel.com](https://vercel.com)
   - Click "Sign Up"
   - Choose "GitHub" and authorize

3. **Import Project**
   - Click "Add New..." → "Project"
   - Select `GymBookingSystem-Web` repository
   - Click "Import"

4. **Configure Project**
   - **Framework Preset**: Vite (auto-detected)
   - **Root Directory**: `./frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist`

5. **Add Environment Variables**
   - Under "Environment Variables", add:
     ```
     VITE_API_URL = https://your-api-domain.azurewebsites.net/api
     ```

6. **Deploy**
   - Click "Deploy"
   - Vercel builds and deploys automatically
   - You get a URL like `https://gym-booking-web.vercel.app`

7. **Setup Auto-Deployments**
   - Every push to `main` branch auto-deploys
   - Pull requests get preview URLs

### Option 2: Vercel CLI

```bash
# Install Vercel CLI
npm install -g vercel

# Navigate to frontend
cd frontend

# Login to Vercel
vercel login

# Deploy to production
vercel --prod

# First time: Vercel asks questions, saves settings
# Future: Just run `vercel --prod` to deploy
```

### Option 3: Manual Build & Upload

```bash
# Build production bundle
cd frontend
npm run build

# Output: dist/ folder ready for hosting
# Upload dist/ contents to any static host (Netlify, AWS S3, Cloudflare Pages, etc.)
```

---

## Backend Deployment (Azure)

Deploy ASP.NET Core API to Azure App Service.

### Prerequisites
- Azure Account (free tier available)
- Azure CLI installed

### Step 1: Create Azure SQL Database

```powershell
# Login to Azure
az login

# Create resource group
az group create --name GymBookingRG --location "East US"

# Create SQL Server
az sql server create `
  --name gymbookingserver `
  --resource-group GymBookingRG `
  --admin-user sqladmin `
  --admin-password "YourComplexPassword123!"

# Create database
az sql db create `
  --resource-group GymBookingRG `
  --server gymbookingserver `
  --name GymBookingDB `
  --sku Basic
```

Get connection string from Azure Portal or:
```powershell
az sql db show-connection-string --name GymBookingDB --server gymbookingserver --client sqlcmd
```

### Step 2: Update Backend Configuration

Edit `GymBookingSystem.API/appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:gymbookingserver.database.windows.net,1433;Initial Catalog=GymBookingDB;Persist Security Info=False;User ID=sqladmin;Password=YourComplexPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "JwtSettings": {
    "Secret": "[GENERATE-RANDOM-64-CHARACTER-STRING-HERE]",
    "Issuer": "gym-booking-api.azurewebsites.net",
    "Audience": "gym-booking-app.vercel.app",
    "ExpiresInHours": 24
  }
}
```

**Generate a secure JWT secret:**
```powershell
# On Windows PowerShell
$random = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$bytes = New-Object byte[] 64
$random.GetBytes($bytes)
$secret = [System.Convert]::ToBase64String($bytes)
Write-Host $secret
```

### Step 3: Create Azure App Service

```powershell
# Create App Service Plan
az appservice plan create `
  --name GymBookingPlan `
  --resource-group GymBookingRG `
  --sku B1 `
  --is-linux

# Create App Service
az webapp create `
  --resource-group GymBookingRG `
  --plan GymBookingPlan `
  --name gym-booking-api `
  --runtime "DOTNET|8.0"
```

### Step 4: Publish Backend

```powershell
cd GymBookingSystem.API

# Build release
dotnet build -c Release

# Publish to folder
dotnet publish -c Release -o ./publish

# Deploy to Azure (install Azure CLI first)
az webapp up --resource-group GymBookingRG --name gym-booking-api --runtime DOTNET:8.0
```

Or use Visual Studio:
1. Right-click project → "Publish"
2. Select "Azure App Service"
3. Follow wizard

### Step 5: Run Database Migration on Azure

```powershell
# Option 1: Azure CLI
az webapp config appsettings set `
  --resource-group GymBookingRG `
  --name gym-booking-api `
  --settings ConnectionStrings__DefaultConnection="your-connection-string"

# Option 2: SSH into app service
# Then run: dotnet ef database update
```

---

## Environment Configuration

### Frontend Environment Variables

**`.env.development`** (local development)
```
VITE_API_URL=http://localhost:5000/api
```

**`.env.production`** (Vercel production)
```
VITE_API_URL=https://gym-booking-api.azurewebsites.net/api
```

Access in code:
```javascript
const apiUrl = import.meta.env.VITE_API_URL;
```

### Backend Environment Variables

**`appsettings.Development.json`** (local)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gym_booking_dev.db"
  }
}
```

**`appsettings.Production.json`** (Azure)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...; Database=GymBookingDB; ..."
  },
  "JwtSettings": {
    "Secret": "[SECURE-64-CHAR-STRING]",
    "Issuer": "gym-booking-api.azurewebsites.net",
    "Audience": "gym-booking-app.vercel.app"
  }
}
```

### Update CORS in Backend

Edit `GymBookingSystem.API/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.WithOrigins(
            "https://gym-booking-app.vercel.app",
            "https://www.gym-booking-app.vercel.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Later in Program.cs
app.UseCors("AllowProduction");
```

---

## Security Checklist

### Before Going Live

- [ ] Change JWT secret (generate new 64-char string)
- [ ] Update database connection string
- [ ] Enable HTTPS on API (Azure does this by default)
- [ ] Configure CORS for your domains only
- [ ] Set strong database password
- [ ] Review all user input validation
- [ ] Test authentication flows end-to-end
- [ ] Check for hardcoded secrets in code
- [ ] Enable API logging and monitoring
- [ ] Set up database backups
- [ ] Test error messages don't expose system details

### Production Checklist

- [ ] SQL Server instead of SQLite
- [ ] Managed Identity for Azure services (don't use connection strings)
- [ ] API rate limiting (add later)
- [ ] Request logging & monitoring
- [ ] Alert on errors
- [ ] Daily database backups
- [ ] Regular security updates
- [ ] SSL/TLS certificate (auto with Azure/Vercel)

---

## Monitoring & Maintenance

### Monitor Deployments

**Vercel**
- Dashboard: https://vercel.com/dashboard
- View logs: Click deployment → Analytics
- Monitor build times and errors

**Azure**
- App Service: Azure Portal
- Monitor → Metrics: CPU, memory, response time
- Logs → App Service Logs

### View Application Logs

**Vercel (Frontend)**
```
No server-side logs (static files)
Check browser console (F12) for client-side errors
```

**Azure (Backend)**
```powershell
# Stream logs in real-time
az webapp log tail --name gym-booking-api --resource-group GymBookingRG

# View recent logs
az webapp log download --name gym-booking-api --resource-group GymBookingRG
```

### Health Checks

Add to your monitoring:

```bash
# Test API health
curl https://gym-booking-api.azurewebsites.net/api/classes

# Test Frontend health
curl https://gym-booking-app.vercel.app
```

### Scaling

- **Frontend**: Vercel auto-scales (no action needed)
- **Backend**: 
  - Monitor CPU/memory in Azure Portal
  - Scale up App Service Plan if needed
  - Consider caching for class listings

### Rollback

**Frontend (Vercel)**
- Go to Deployments tab
- Click on previous deployment
- Click "Redeploy"

**Backend (Azure)**
```powershell
# Rollback to previous deployment
az webapp deployment slot swap --resource-group GymBookingRG --name gym-booking-api
```

---

## Cost Estimates

### Azure (Monthly)
- **App Service (B1 Plan)**: ~$10/month
- **SQL Database (Basic)**: ~$5/month
- **Storage**: Minimal
- **Total**: ~$15/month (free tier available for first 12 months)

### Vercel (Monthly)
- **Free Tier**: $0 (unlimited for hobby projects)
- **Pro Tier**: $20/month (needed for production with custom domains)
- **Total**: $0-20/month

### Total Production Cost: ~$15-35/month

---

## Troubleshooting

### "503 Service Unavailable" on API

```
Check: App Service is running
  az webapp show --name gym-booking-api --query state
Restart: az webapp restart --name gym-booking-api
```

### "CORS Error" in Frontend

```
Check: CORS policy in Program.cs
Add API domain to CORS origins
Redeploy backend
```

### Frontend Can't Connect to API

```
Check: API URL in .env.production
Verify: API is responding (curl the URL)
Check: Network tab in browser DevTools (F12)
```

### Database Connection Error

```
Check: Connection string in appsettings.Production.json
Verify: SQL Server firewall allows app service
Test: Connect with Azure Data Studio locally
```

### Build Failures on Vercel

```
Check: package.json has all dependencies
Run locally: npm install && npm run build
Check: Build logs in Vercel dashboard
```

---

## Next Steps

1. **Deploy frontend to Vercel** → Get production URL
2. **Deploy backend to Azure** → Get API URL
3. **Update environment variables** → Point frontend to backend
4. **Test all flows** in production
5. **Setup monitoring** → Watch logs and metrics
6. **Configure backups** → Database daily backups
7. **Plan scaling** → Monitor usage patterns

---

**Deployment Status**: ✅ Ready to deploy

All code is production-ready. Follow this guide step-by-step for a reliable, scalable deployment.

*Last updated: July 3, 2026*
