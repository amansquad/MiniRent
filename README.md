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

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: MS SQL Server (with GUID IDs)
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Authentication**: JWT (JSON Web Tokens)
- **Documentation**: Swagger/OpenAPI with XML Comments

## 📺 Feature Tour (Demo)

### 🏘️ Property Discovery
- **Modern Landing Page**: Vibrant hero section and featured properties with smooth micro-animations.
- **Advanced Search Bar**: Global search enabled across properties, tenants, and inquiries.
- **Interactive Map**: View property locations and neighborhood details.

### 💼 Management Dashboard
- **Owner Dashboard**: Track monthly revenue, active rentals, and pending inquiries in a unified view.
- **Rental History**: View a complete timeline of previous tenants and payments for any listing.
- **Inquiry Handling**: Direct messaging system for quick communication between owners and prospective tenants.

### 🔒 Security & Performance
- **Role-Based Access**: Specialized views for Admins, Agents, and Tenants.
- **Global Error Handling**: Standardized API responses with structured logging.
- **High Performance**: Optimized with response caching and Brotli/Gzip compression.

## 🛠️ Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/) (v18+)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MS SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### 1. Database Setup
The system uses MS SQL Server and string-based GUIDs for all IDs.
- Ensure SQL Server is running.
- The backend will automatically apply migrations and seed initial data on the first run.

### 2. Backend Configuration
Create or update `Backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniRent;User Id=sa;Password=your_password;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SUPER_SECRET_KEY_HERE",
    "Issuer": "MiniRent",
    "Audience": "MiniRentUsers"
  }
}
```

### 3. Frontend Configuration
Create a `.env.local` file in the root directory:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_APP_NAME="MiniRent"
```

### 4. Running the Application

**Backend:**
```bash
cd Backend
dotnet restore
dotnet ef database update
dotnet run
```
*The API will be available at `http://localhost:5000`*

**Frontend:**
```bash
# From the root directory
npm install
npm run dev
```
*The Web App will be available at `http://localhost:3000` (or `5173`)*

## 📖 API Documentation
MiniRent includes a professional Swagger UI with detailed endpoint descriptions and schema models.
- **Swagger UI**: Visit `http://localhost:5000/swagger` when the backend is running.
- **XML Comments**: All core endpoints are documented with parameters and return types.

## 🧪 Quality Assurance
We maintain a high standard of code quality with automated tests.
- **Unit Tests**: Powered by xUnit, Moq, and FluentAssertions.
- **Run Tests**:
  ```bash
  dotnet test Tests/Tests.csproj
  ```

## 📜 Default Credentials
- **Username**: `admin`
- **Password**: `admin123`
- *Note: These are automatically seeded for development environments.*


