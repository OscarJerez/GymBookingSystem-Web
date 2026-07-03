# Quick Start - Deployment

## 🚀 Deploy Frontend to Vercel (5 minutes)

```bash
# Option 1: GitHub Integration (Easiest)
1. Go to vercel.com
2. Click "Add New Project"
3. Select GymBookingSystem-Web repo
4. Set root directory: frontend
5. Add env var: VITE_API_URL = https://your-api.azurewebsites.net/api
6. Click Deploy ✅

# Option 2: Vercel CLI
npm install -g vercel
cd frontend
vercel --prod
```

**Result**: https://gym-booking-web.vercel.app (or your custom domain)

---

## 🔧 Deploy Backend to Azure (15 minutes)

```bash
# 1. Create resources
az group create --name GymBookingRG --location "East US"
az sql server create --name gymbookingserver --resource-group GymBookingRG --admin-user sqladmin --admin-password "Password123!"
az sql db create --resource-group GymBookingRG --server gymbookingserver --name GymBookingDB --sku Basic
az appservice plan create --name GymBookingPlan --resource-group GymBookingRG --sku B1 --is-linux
az webapp create --resource-group GymBookingRG --plan GymBookingPlan --name gym-booking-api --runtime "DOTNET|8.0"

# 2. Build and publish
cd GymBookingSystem.API
dotnet publish -c Release -o ./publish

# 3. Deploy
az webapp up --resource-group GymBookingRG --name gym-booking-api --runtime DOTNET:8.0
```

**Result**: https://gym-booking-api.azurewebsites.net/api

---

## ✅ Final Checklist

- [ ] Change JWT secret in `appsettings.Production.json`
- [ ] Update DB connection string in `appsettings.Production.json`
- [ ] Update CORS origins in `Program.cs` (your Vercel domain)
- [ ] Set `VITE_API_URL` in Vercel environment variables
- [ ] Test login/booking/cancel flows
- [ ] Monitor logs in Azure Portal & Vercel Dashboard
- [ ] Setup daily database backups

---

**You're live!** 🎉

Frontend: https://gym-booking-web.vercel.app
API: https://gym-booking-api.azurewebsites.net/api

---

See [DEPLOYMENT.md](./DEPLOYMENT.md) for detailed instructions.
