# Statistics Tables and Views Documentation

This document describes the statistics tables and views created for tracking user ownership, property performance, and rental statistics.

## Statistics Tables

### 1. UserOwnershipStats
Tracks property ownership and rental statistics for each user.

**Columns:**
- `UserId` - Reference to the user
- `TotalProperties` - Total number of properties owned
- `AvailableProperties` - Properties with Available status
- `RentedProperties` - Properties with Rented status
- `ReservedProperties` - Properties with Reserved status
- `MaintenanceProperties` - Properties with Maintenance status
- `TotalRentalsAsTenant` - Total rentals where user is the tenant
- `ActiveRentalsAsTenant` - Active rentals as tenant
- `EndedRentalsAsTenant` - Ended rentals as tenant
- `TotalMonthlyIncome` - Sum of monthly rent from rented properties
- `LastUpdated` - Timestamp of last update

### 2. PropertyStatistics
Tracks performance metrics for each property.

**Columns:**
- `PropertyId` - Reference to the property
- `TotalRentals` - Total number of rental records
- `ActiveRentals` - Currently active rentals
- `TotalInquiries` - Total inquiries received
- `PendingInquiries` - Inquiries awaiting response
- `TotalReviews` - Number of reviews
- `AverageRating` - Average rating from reviews
- `TotalRevenue` - Total revenue generated
- `OccupancyRate` - Percentage of time property is rented
- `LastRentalDate` - Date of most recent rental
- `LastUpdated` - Timestamp of last update

### 3. RentalStatistics
Tracks payment statistics for each rental.

**Columns:**
- `RentalId` - Reference to the rental record
- `TotalPayments` - Total number of payments
- `PaidPayments` - Number of paid payments
- `PendingPayments` - Number of pending payments
- `OverduePayments` - Number of overdue payments
- `TotalPaid` - Sum of paid amounts
- `TotalPending` - Sum of pending amounts
- `TotalOverdue` - Sum of overdue amounts
- `PaymentCompletionRate` - Percentage of payments completed
- `LastPaymentDate` - Date of most recent payment
- `NextPaymentDue` - Date of next payment due
- `LastUpdated` - Timestamp of last update

## Database Views

### 1. vw_UserOwnership
Real-time view of user ownership statistics combining data from Users, Properties, and RentalRecords.

**Use Case:** Dashboard showing user portfolio overview

### 2. vw_PropertyDetails
Comprehensive property information with aggregated statistics.

**Includes:**
- Property basic info (title, address, status, rent)
- Owner information
- Rental history counts
- Inquiry statistics
- Review metrics
- Image and amenity counts

**Use Case:** Property detail pages, property listings with stats

### 3. vw_RentalDetails
Detailed rental information with payment statistics.

**Includes:**
- Rental status and dates
- Property information
- Tenant information
- Owner information
- Payment breakdown (paid, pending, overdue)
- Payment completion rate

**Use Case:** Rental management dashboard, tenant payment tracking

### 4. vw_PaymentSummary
Payment-centric view with related rental, property, and user information.

**Includes:**
- Payment details (amount, status, dates)
- Calculated fields (days overdue, days until due)
- Related rental, property, tenant, and owner info

**Use Case:** Payment tracking, overdue payment reports

### 5. vw_InquirySummary
Inquiry tracking with property and owner information.

**Includes:**
- Inquirer contact information
- Property details
- Owner information
- Conversion status (if converted to rental)
- Response time metrics

**Use Case:** Lead management, inquiry response tracking

### 6. vw_ActivitySummary
User activity log with time-based grouping.

**Includes:**
- Activity type and description
- User information
- Time groupings (day, week, month, year)

**Use Case:** Activity reports, user behavior analysis

## API Endpoints

### Statistics Controller (`/api/statistics`)

**User Ownership:**
- `GET /api/statistics/users/{userId}` - Get stats for specific user
- `GET /api/statistics/users` - Get stats for all users
- `POST /api/statistics/users/{userId}/refresh` - Refresh user stats

**Property Statistics:**
- `GET /api/statistics/properties/{propertyId}` - Get stats for specific property
- `GET /api/statistics/properties` - Get stats for all properties
- `POST /api/statistics/properties/{propertyId}/refresh` - Refresh property stats

**Rental Statistics:**
- `GET /api/statistics/rentals/{rentalId}` - Get stats for specific rental
- `GET /api/statistics/rentals` - Get stats for all rentals
- `POST /api/statistics/rentals/{rentalId}/refresh` - Refresh rental stats

**Bulk Operations:**
- `POST /api/statistics/refresh-all` - Refresh all statistics

## Usage Examples

### Get User Ownership Stats
```http
GET /api/statistics/users/00000000-0000-0000-0000-000000000001
```

**Response:**
```json
{
  "userId": "00000000-0000-0000-0000-000000000001",
  "username": "admin",
  "fullName": "System Administrator",
  "totalPropertiesOwned": 5,
  "availableProperties": 2,
  "rentedProperties": 3,
  "totalMonthlyIncome": 4500.00,
  "totalRentalsAsTenant": 0,
  "activeRentalsAsTenant": 0
}
```

### Get Property Statistics
```http
GET /api/statistics/properties/{propertyId}
```

**Response:**
```json
{
  "propertyId": "...",
  "title": "Modern Downtown Apartment",
  "address": "123 Main St",
  "status": "Rented",
  "totalRentals": 3,
  "activeRentals": 1,
  "totalInquiries": 15,
  "pendingInquiries": 2,
  "totalReviews": 5,
  "averageRating": 4.5
}
```

### Refresh Statistics
```http
POST /api/statistics/users/{userId}/refresh
```

## Maintenance

Statistics tables should be refreshed:
- After creating/updating properties
- After creating/updating rentals
- After payment status changes
- Periodically via scheduled job (recommended: daily)

Use the refresh endpoints to update statistics on-demand or set up a background job to refresh all statistics regularly.
