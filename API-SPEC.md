# API Specification - Gym Booking System

## Base URL
```
http://localhost:5000/api
```

## Authentication
All protected endpoints require JWT Bearer token in the `Authorization` header:
```
Authorization: Bearer <token>
```

Tokens are obtained from the login endpoint and contain user claims (id, username, email, role).

---

## Auth Endpoints

### POST /auth/register
Register a new user (default role: Member).

**Request:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Response (200):**
```json
{
  "id": 3,
  "username": "newuser",
  "email": "new@example.com",
  "role": "Member",
  "token": "eyJhbGc..."
}
```

**Error Responses:**
- 400: Username or email already exists
- 400: Invalid input

---

### POST /auth/login
Login and get JWT token.

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response (200):**
```json
{
  "id": 1,
  "username": "admin",
  "email": "admin@gymbooking.com",
  "role": "Admin",
  "token": "eyJhbGc..."
}
```

**Error Responses:**
- 401: Invalid credentials
- 401: Account is inactive

---

## Classes Endpoints

### GET /classes
Get all active classes.

**Response (200):**
```json
[
  {
    "id": 1,
    "name": "Morning Yoga",
    "description": "Relaxing yoga session to start your day",
    "startTime": "2026-07-04T06:00:00Z",
    "endTime": "2026-07-04T07:00:00Z",
    "capacity": 20,
    "bookedCount": 5,
    "availableSpots": 15,
    "instructorName": "Sarah",
    "status": "Spots left: 15",
    "timeRange": "Friday 06:00 - 07:00"
  }
]
```

---

### GET /classes/{id}
Get a specific class.

**Response (200):** Same as single item above

**Error Responses:**
- 404: Class not found

---

### POST /classes
Create a new class (Owner/Admin only).

**Request:**
```json
{
  "name": "string",
  "description": "string",
  "startTime": "2026-07-04T18:00:00Z",
  "endTime": "2026-07-04T19:00:00Z",
  "capacity": 20,
  "instructorName": "string"
}
```

**Response (201):** Full class object

**Error Responses:**
- 400: Start time must be before end time
- 401: Unauthorized
- 403: Forbidden (not Owner/Admin)

---

### PUT /classes/{id}
Update a class (Owner/Admin only).

**Request:** Same as POST

**Response (200):** Updated class object

**Error Responses:**
- 404: Class not found
- 400: Invalid time range
- 401: Unauthorized

---

### DELETE /classes/{id}
Soft delete a class (Owner/Admin only).

**Response (204):** No content

**Error Responses:**
- 404: Class not found
- 401: Unauthorized

---

## Bookings Endpoints

### GET /bookings
Get logged-in user's active bookings (Members only).

**Response (200):**
```json
[
  {
    "id": 1,
    "userId": 3,
    "classId": 2,
    "className": "HIIT Training",
    "bookedAt": "2026-07-03T18:30:00Z",
    "status": "Active"
  }
]
```

**Error Responses:**
- 401: Unauthorized

---

### GET /bookings/all
Get all bookings in the system (Admin only).

**Response (200):** Array of all booking objects

**Error Responses:**
- 401: Unauthorized
- 403: Forbidden (not Admin)

---

### POST /bookings
Book a class for the logged-in user.

**Request:**
```json
{
  "classId": 1
}
```

**Response (201):** Booking object created

**Error Responses:**
- 404: Class not found
- 400: Class is full
- 400: Already booked for this class
- 401: Unauthorized

---

### DELETE /bookings/{id}
Cancel a booking (user can cancel their own, Admin can cancel any).

**Response (204):** No content

**Error Responses:**
- 404: Booking not found
- 401: Unauthorized
- 403: Forbidden (not your booking and not Admin)

---

## Error Response Format

All errors return:
```json
{
  "message": "Human-readable error message"
}
```

With appropriate HTTP status codes:
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

---

## Status Codes Summary

| Code | Meaning |
|------|---------|
| 200 | Success - Request succeeded |
| 201 | Created - Resource created successfully |
| 204 | No Content - Success, no response body |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - No/invalid token |
| 403 | Forbidden - Valid token but no permission |
| 404 | Not Found - Resource doesn't exist |
| 500 | Server Error |

---

## Roles & Permissions

| Endpoint | Member | Owner | Admin |
|----------|--------|-------|-------|
| POST /auth/register | ✅ | ✅ | ✅ |
| POST /auth/login | ✅ | ✅ | ✅ |
| GET /classes | ✅ | ✅ | ✅ |
| GET /classes/{id} | ✅ | ✅ | ✅ |
| POST /classes | ❌ | ✅ | ✅ |
| PUT /classes/{id} | ❌ | ✅ | ✅ |
| DELETE /classes/{id} | ❌ | ✅ | ✅ |
| GET /bookings | ✅ | ❌ | ❌ |
| GET /bookings/all | ❌ | ❌ | ✅ |
| POST /bookings | ✅ | ❌ | ❌ |
| DELETE /bookings/{id} | ✅* | ❌ | ✅ |

*Members can only cancel their own bookings

---

## Example Workflow

### 1. New user registers
```
POST /auth/register
→ 201 with token
```

### 2. User logs in
```
POST /auth/login
→ 200 with token
```

### 3. User views classes
```
GET /classes (with token in header)
→ 200 list of classes
```

### 4. User books a class
```
POST /bookings {"classId": 2}
→ 201 booking created
```

### 5. User views their bookings
```
GET /bookings (with token)
→ 200 list of user's bookings
```

### 6. User cancels booking
```
DELETE /bookings/1 (with token)
→ 204 booking cancelled
```

---

## Rate Limiting
Currently: None. Add later for production.

## Pagination
Currently: None. Add if many classes exist.

## Filtering
Currently: All classes endpoint returns active only. Extend if needed.
