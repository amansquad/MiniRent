# MiniRent 🏠

MiniRent is a comprehensive property rental management system designed to streamline the connection between property owners and tenants. It features a modern, responsive interface for property discovery, rental tracking, and inquiry management with advanced analytics and statistics.

## 🚀 Tech Stack

### Frontend
- **Framework**: Next.js 15+ (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: Shadcn UI (Radix UI)
- **State Management**: Zustand
- **Icons**: Lucide React
- **Animations**: Framer Motion
- **Image Management**: Next.js Image Optimization

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Database**: MS SQL Server (with GUID IDs)
- **ORM**: Entity Framework Core 8.0
- **Mapping**: AutoMapper
- **Authentication**: JWT (JSON Web Tokens)
- **Documentation**: Swagger/OpenAPI with XML Comments
- **Middleware**: Custom Exception Handling

## ✨ Key Features

### 🏘️ Property Management
- **Property Listings**: Create, update, and manage property listings with detailed information
- **Image Management**: Upload and manage multiple property images with drag-and-drop interface
- **Property Search**: Advanced search and filtering capabilities
- **Property Statistics**: Track views, inquiries, and rental performance
- **Amenities**: Comprehensive amenity management system

### 💼 Ownership & Rentals
- **Ownership Tracking**: Multi-owner property support with ownership percentages
- **Rental Records**: Complete rental history and active rental management
- **Payment Tracking**: Monitor payments and rental income
- **Rental Inquiries**: Handle tenant inquiries and applications
- **Activity Logs**: Track all property-related activities

### 📊 Analytics & Statistics
- **Dashboard Analytics**: Real-time statistics for owners and administrators
- **Property Statistics**: Individual property performance metrics
- **Rental Statistics**: Revenue tracking and occupancy rates
- **User Statistics**: Owner portfolio performance and insights
- **Database Views**: Optimized SQL views for complex reporting

### 👥 User Management
- **Role-Based Access Control**: Admin, Owner, and Tenant roles
- **User Authentication**: Secure JWT-based authentication
- **User Profiles**: Comprehensive user information management
- **Activity Tracking**: Monitor user actions and engagement

### 🔍 Search & Discovery
- **Global Search**: Search across properties, locations, and amenities
- **Advanced Filters**: Filter by price, location, property type, and more
- **Featured Properties**: Highlight premium listings
- **Review System**: Property ratings and tenant reviews

### 🔒 Security & Performance
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Granular access control
- **Global Error Handling**: Standardized API responses with structured logging
- **Database Indexing**: Optimized queries for high performance
- **Response Caching**: Improved API response times
- **Compression**: Brotli/Gzip compression enabled

## 🛠️ Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/) (v18+)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MS SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### 1. Database Setup
The system uses MS SQL Server with string-based GUIDs for all IDs.

**Option A: Automatic Setup (Recommended)**
- Ensure SQL Server is running
- The backend will automatically apply migrations and seed initial data on first run
- An admin user will be created automatically

**Option B: Manual Setup**
```bash
cd Backend
dotnet ef database update
# Optional: Run SQL scripts for additional setup
sqlcmd -S localhost -d MiniRent -i check-and-create-admin.sql
```

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

MiniRent includes comprehensive Swagger/OpenAPI documentation with detailed endpoint descriptions.

**Access Swagger UI**: `http://localhost:5000/swagger`

### Available Endpoints
- **Authentication**: `/api/auth` - Login, register, token management
- **Properties**: `/api/properties` - CRUD operations for properties
- **Ownership**: `/api/ownership` - Property ownership management
- **Rentals**: `/api/rentals` - Rental records and management
- **Inquiries**: `/api/inquiries` - Tenant inquiry handling
- **Reviews**: `/api/reviews` - Property reviews and ratings
- **Amenities**: `/api/amenities` - Amenity management
- **Search**: `/api/search` - Advanced property search
- **Dashboard**: `/api/dashboard` - Analytics and statistics
- **Statistics**: `/api/statistics` - Detailed performance metrics
- **Users**: `/api/users` - User management (Admin only)

## 🗂️ Project Structure

```
MiniRent/
├── Backend/                    # ASP.NET Core API
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic layer
│   ├── Models/                # Entity models
│   ├── Dtos/                  # Data transfer objects
│   ├── Data/                  # Database context
│   ├── Migrations/            # EF Core migrations
│   ├── Middleware/            # Custom middleware
│   └── Config/                # AutoMapper profiles
├── src/                       # Next.js frontend
│   ├── app/                   # App router pages
│   ├── components/            # React components
│   ├── lib/                   # Utilities and helpers
│   └── stores/                # Zustand state management
└── public/                    # Static assets
```

## 🔧 Development

### Backend Development
```bash
cd Backend
dotnet watch run              # Hot reload enabled
dotnet ef migrations add MigrationName  # Create migration
dotnet ef database update     # Apply migrations
```

### Frontend Development
```bash
npm run dev                   # Development server
npm run build                 # Production build
npm run lint                  # Run ESLint
npm run type-check            # TypeScript validation
```

## 🐳 Docker Support

The project includes Docker configuration for containerized deployment:

```bash
docker-compose up -d          # Start all services
docker-compose down           # Stop all services
```

## 📜 Default Credentials

**Admin Account** (auto-created on first run):
- **Username**: `admin`
- **Password**: `admin123`

**Important**: Change these credentials in production environments!

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with Next.js and ASP.NET Core
- UI components from Shadcn UI
- Icons from Lucide React


