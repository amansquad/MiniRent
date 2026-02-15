# MiniRent 🏠

MiniRent is a comprehensive property rental management system designed to streamline the connection between property owners and tenants. It features a modern, responsive interface for property discovery, rental tracking, and inquiry management.

## 🚀 Tech Stack

### Frontend
- **Framework**: Next.js 16 (App Router)
- **Styling**: Tailwind CSS
- **UI Components**: Shadcn UI (Radix UI)
- **State Management**: Zustand
- **Icons**: Lucide React
- **Animations**: Framer Motion
- **Notifications**: Sonner / Shadcn Toast

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Authentication**: JWT (JSON Web Tokens)
- **Hashing**: BCrypt.net

## ✨ Key Features

- **Property Management**: Complete CRUD operations for properties with rich details (address, area, bedrooms, floor, rent).
- **Rental Lifecycle**: 
    - Automatic status transitions (Available ↔ Rented).
    - Manual overrides for "Reserved" and "Maintenance" statuses.
    - Rental history tracking for each property.
- **Inquiry System**: Tenants can send inquiries; owners can reply directly from their dashboard.
- **Advanced Search**: Global search with highlighting and deep linking to specific properties, rentals, or inquiries.
- **Smart Filtering**: Filter by status, price range, bedrooms, and address keyword.
- **Role-Based Access**: 
    - **Users**: Manage their own properties, rentals, and inquiries.
    - **Admins**: Full control over all properties, rentals, and users.

## 📂 Project Structure

```text
MiniRent/
├── Backend/                 # ASP.NET Core Web API
│   ├── Controllers/         # API Endpoints
│   ├── Data/                # DbContext and Migrations
│   ├── Dtos/                # Data Transfer Objects
│   ├── Models/              # EF Core Entities
│   ├── Services/            # Business Logic
│   └── Program.cs           # App Configuration
├── src/                     # Next.js Frontend
│   ├── app/                 # Routes and API Handlers
│   ├── components/          # Reusable UI Components
│   └── lib/                 # Utilities and Hooks
├── public/                  # Static Assets
└── package.json             # Frontend Dependencies & Scripts
```

## 🛠️ Getting Started

### Prerequisites
- Node.js (v18+)
- .NET 8.0 SDK
- PostgreSQL

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd MiniRent
   ```

2. **Database Configuration**
   Update the connection string in `Backend/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=MiniRent;Username=postgres;Password=your_password"
   }
   ```

3. **Backend Setup**
   ```bash
   cd Backend
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

4. **Frontend Setup**
   ```bash
   # From the root directory
   npm install
   npm run dev
   ```

## 📜 Usage

- **Default Admin Login**:
  - **Username**: `admin`
  - **Password**: `admin123`
- The system will automatically seed an admin account if the database is empty.
- Use the **Global Search** in the navbar to find any item by ID or keyword.
- Access **My Properties** to manage your listings and view rental requests.

## 📄 License
MIT License
