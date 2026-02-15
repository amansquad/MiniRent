# MiniRent - Apartment & Property Rental Management System

## Project Overview
MiniRent is a full-stack web application for managing apartment rentals, designed as an evaluation project for DAFTech Social Solution. The system supports property listing, rental tracking, and inquiry management for small real estate offices.

## Technology Stack
- **Backend:** .NET 8 (ASP.NET Core Web API) with Entity Framework Core and SQL Server
- **Frontend:** Next.js (React) with TypeScript, Tailwind CSS, and shadcn/ui
- **Database / ORM:** Prisma (PostgreSQL) for the app schema found in `prisma/schema.prisma`; the backend also includes EF Core mappings for SQL Server. See the repository for which service uses which DB.
- **Authentication:** JWT (JSON Web Tokens)

## Features
- User authentication and role-based access
- Property management (CRUD operations)
- Rental tracking and management
- Inquiry/lead management
- Dashboard with key metrics
- Global search functionality

## Project Structure (top-level)
```
MiniRent/
├── Backend/                    # ASP.NET Core Web API (C#)
├── prisma/                     # Prisma schema and migrations (PostgreSQL)
├── src/                        # Next.js frontend (React + TypeScript)
├── public/                     # Static assets for frontend
├── package.json                # Frontend scripts & dependencies
└── README.md
```

## Setup Instructions

### Backend Setup (ASP.NET Core)
1. Navigate to the `Backend` directory
2. Restore NuGet packages: `dotnet restore`
3. Update the database connection string in `appsettings.json` (if using SQL Server)
4. Apply Entity Framework migrations: `dotnet ef database update`
5. Run the backend: `dotnet run`

### Frontend Setup (Next.js)
1. From repository root, install dependencies: `npm install`
2. Run the development server: `npm run dev`
3. Useful scripts (from `package.json`):
   - `npm run dev` — start Next.js dev server (port 3000)
   - `npm run build` — build for production
   - `npm run start` — start production server (after build)
   - `npm run db:migrate` / `npm run db:generate` — Prisma helpers

### Database Notes
The repository contains a Prisma schema (`prisma/schema.prisma`) configured for PostgreSQL. The Backend project is configured to use Entity Framework Core with SQL Server. Depending on which service you're running, ensure the appropriate `DATABASE_URL` or `appsettings.json` connection string is set.

## Database Schema
Main entities present in the Prisma schema include:
- `User` (roles: ADMIN, MANAGER, USER)
- `Property` and `PropertyImage`
- `Rental` and `Payment`
- `Inquiry`
- `Activity`


## API Endpoints

### Authentication
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/profile

### Properties
- GET /api/properties (with pagination and filters)
- GET /api/properties/{id}
- POST /api/properties
- PUT /api/properties/{id}
- DELETE /api/properties/{id} (soft delete)
- PATCH /api/properties/{id}/status

### Rentals
- GET /api/rentals
- GET /api/rentals/{id}
- POST /api/rentals
- PUT /api/rentals/{id}
- PATCH /api/rentals/{id}/end

### Inquiries
- GET /api/inquiries
- GET /api/inquiries/{id}
- POST /api/inquiries
- PUT /api/inquiries/{id}
- DELETE /api/inquiries/{id}

### Dashboard
- GET /api/dashboard/overview
- GET /api/dashboard/search

## Security
- JWT authentication with 1-hour expiration
- Role-based authorization (Admin/Agent)
- Password hashing with BCrypt
- Input validation and XSS prevention

## Testing
- Unit tests with xUnit (included in test projects)
- Integration tests for API endpoints
- Frontend component tests with Jasmine/Karma

## Deployment
- Local development setup
- Optional Docker configuration available
- Production deployment scripts included

## Evaluation Criteria
This project is evaluated based on:
- Completeness (30 points)
- Code Quality & Structure (20 points)
- Database & ORM (15 points)
- Frontend Implementation (15 points)
- Security & Best Practices (10 points)
- Testing & Documentation (5 points)
- Innovation/Bonus (5 points)

**Total: 100 points | Passing Threshold: 70+ points**

## Contact
For questions during implementation, contact DAFTech Social Solution evaluation team.
