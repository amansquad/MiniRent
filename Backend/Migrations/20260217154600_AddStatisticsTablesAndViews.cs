using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniRent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticsTablesAndViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create UserOwnershipStats table
            migrationBuilder.Sql(@"
                CREATE TABLE UserOwnershipStats (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    UserId UNIQUEIDENTIFIER NOT NULL,
                    TotalProperties INT NOT NULL DEFAULT 0,
                    AvailableProperties INT NOT NULL DEFAULT 0,
                    RentedProperties INT NOT NULL DEFAULT 0,
                    ReservedProperties INT NOT NULL DEFAULT 0,
                    MaintenanceProperties INT NOT NULL DEFAULT 0,
                    TotalRentalsAsTenant INT NOT NULL DEFAULT 0,
                    ActiveRentalsAsTenant INT NOT NULL DEFAULT 0,
                    EndedRentalsAsTenant INT NOT NULL DEFAULT 0,
                    TotalMonthlyIncome DECIMAL(18,2) NOT NULL DEFAULT 0,
                    LastUpdated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_UserOwnershipStats_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );
                
                CREATE UNIQUE INDEX IX_UserOwnershipStats_UserId ON UserOwnershipStats(UserId);
            ");

            // Create PropertyStatistics table
            migrationBuilder.Sql(@"
                CREATE TABLE PropertyStatistics (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    PropertyId UNIQUEIDENTIFIER NOT NULL,
                    TotalRentals INT NOT NULL DEFAULT 0,
                    ActiveRentals INT NOT NULL DEFAULT 0,
                    TotalInquiries INT NOT NULL DEFAULT 0,
                    PendingInquiries INT NOT NULL DEFAULT 0,
                    TotalReviews INT NOT NULL DEFAULT 0,
                    AverageRating DECIMAL(3,2) NULL,
                    TotalRevenue DECIMAL(18,2) NOT NULL DEFAULT 0,
                    OccupancyRate DECIMAL(5,2) NOT NULL DEFAULT 0,
                    LastRentalDate DATETIME2 NULL,
                    LastUpdated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_PropertyStatistics_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE
                );
                
                CREATE UNIQUE INDEX IX_PropertyStatistics_PropertyId ON PropertyStatistics(PropertyId);
            ");

            // Create RentalStatistics table
            migrationBuilder.Sql(@"
                CREATE TABLE RentalStatistics (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    RentalId UNIQUEIDENTIFIER NOT NULL,
                    TotalPayments INT NOT NULL DEFAULT 0,
                    PaidPayments INT NOT NULL DEFAULT 0,
                    PendingPayments INT NOT NULL DEFAULT 0,
                    OverduePayments INT NOT NULL DEFAULT 0,
                    TotalPaid DECIMAL(18,2) NOT NULL DEFAULT 0,
                    TotalPending DECIMAL(18,2) NOT NULL DEFAULT 0,
                    TotalOverdue DECIMAL(18,2) NOT NULL DEFAULT 0,
                    PaymentCompletionRate DECIMAL(5,2) NOT NULL DEFAULT 0,
                    LastPaymentDate DATETIME2 NULL,
                    NextPaymentDue DATETIME2 NULL,
                    LastUpdated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    CONSTRAINT FK_RentalStatistics_RentalRecords FOREIGN KEY (RentalId) REFERENCES RentalRecords(Id) ON DELETE CASCADE
                );
                
                CREATE UNIQUE INDEX IX_RentalStatistics_RentalId ON RentalStatistics(RentalId);
            ");

            // Create UserOwnershipView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserOwnership AS
                SELECT 
                    u.Id AS UserId,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    -- Property ownership stats
                    COUNT(DISTINCT p.Id) AS TotalPropertiesOwned,
                    COUNT(DISTINCT CASE WHEN p.Status = 'Available' THEN p.Id END) AS AvailableProperties,
                    COUNT(DISTINCT CASE WHEN p.Status = 'Rented' THEN p.Id END) AS RentedProperties,
                    COUNT(DISTINCT CASE WHEN p.Status = 'Reserved' THEN p.Id END) AS ReservedProperties,
                    COUNT(DISTINCT CASE WHEN p.Status = 'Maintenance' THEN p.Id END) AS MaintenanceProperties,
                    ISNULL(SUM(CASE WHEN p.Status = 'Rented' THEN p.MonthlyRent ELSE 0 END), 0) AS TotalMonthlyIncome,
                    -- Rental as tenant stats
                    COUNT(DISTINCT rr.Id) AS TotalRentalsAsTenant,
                    COUNT(DISTINCT CASE WHEN rr.Status = 'Active' THEN rr.Id END) AS ActiveRentalsAsTenant,
                    COUNT(DISTINCT CASE WHEN rr.Status = 'Ended' THEN rr.Id END) AS EndedRentalsAsTenant,
                    COUNT(DISTINCT CASE WHEN rr.Status = 'Pending' THEN rr.Id END) AS PendingRentalsAsTenant,
                    ISNULL(SUM(CASE WHEN rr.Status = 'Active' THEN rr.MonthlyRent ELSE 0 END), 0) AS TotalMonthlyExpense
                FROM Users u
                LEFT JOIN Properties p ON u.Id = p.CreatedById AND p.IsDeleted = 0
                LEFT JOIN RentalRecords rr ON u.Id = rr.TenantId
                GROUP BY u.Id, u.Username, u.FullName, u.Email, u.Role;
            ");

            // Create PropertyDetailsView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_PropertyDetails AS
                SELECT 
                    p.Id AS PropertyId,
                    p.Title,
                    p.Address,
                    p.City,
                    p.State,
                    p.Country,
                    p.PropertyType,
                    p.Status,
                    p.MonthlyRent,
                    p.Bedrooms,
                    p.Bathrooms,
                    p.Area,
                    -- Owner info
                    u.Id AS OwnerId,
                    u.Username AS OwnerUsername,
                    u.FullName AS OwnerName,
                    u.Email AS OwnerEmail,
                    -- Statistics
                    COUNT(DISTINCT rr.Id) AS TotalRentals,
                    COUNT(DISTINCT CASE WHEN rr.Status = 'Active' THEN rr.Id END) AS ActiveRentals,
                    COUNT(DISTINCT ri.Id) AS TotalInquiries,
                    COUNT(DISTINCT CASE WHEN ri.Status = 'Pending' THEN ri.Id END) AS PendingInquiries,
                    COUNT(DISTINCT rev.Id) AS TotalReviews,
                    AVG(CAST(rev.Rating AS DECIMAL(3,2))) AS AverageRating,
                    COUNT(DISTINCT pi.Id) AS TotalImages,
                    COUNT(DISTINCT pa.AmenitiesId) AS TotalAmenities,
                    MAX(rr.StartDate) AS LastRentalDate,
                    p.CreatedAt,
                    p.UpdatedAt
                FROM Properties p
                LEFT JOIN Users u ON p.CreatedById = u.Id
                LEFT JOIN RentalRecords rr ON p.Id = rr.PropertyId
                LEFT JOIN RentalInquiries ri ON p.Id = ri.PropertyId
                LEFT JOIN Reviews rev ON p.Id = rev.PropertyId
                LEFT JOIN PropertyImages pi ON p.Id = pi.PropertyId
                LEFT JOIN PropertyAmenities pa ON p.Id = pa.PropertiesId
                WHERE p.IsDeleted = 0
                GROUP BY 
                    p.Id, p.Title, p.Address, p.City, p.State, p.Country, 
                    p.PropertyType, p.Status, p.MonthlyRent, p.Bedrooms, 
                    p.Bathrooms, p.Area, u.Id, u.Username, u.FullName, 
                    u.Email, p.CreatedAt, p.UpdatedAt;
            ");

            // Create RentalDetailsView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_RentalDetails AS
                SELECT 
                    rr.Id AS RentalId,
                    rr.Status AS RentalStatus,
                    rr.StartDate,
                    rr.EndDate,
                    rr.MonthlyRent,
                    rr.SecurityDeposit,
                    -- Property info
                    p.Id AS PropertyId,
                    p.Title AS PropertyTitle,
                    p.Address AS PropertyAddress,
                    p.City,
                    p.State,
                    -- Tenant info
                    t.Id AS TenantId,
                    t.Username AS TenantUsername,
                    t.FullName AS TenantName,
                    t.Email AS TenantEmail,
                    t.Phone AS TenantPhone,
                    -- Owner info
                    o.Id AS OwnerId,
                    o.Username AS OwnerUsername,
                    o.FullName AS OwnerName,
                    -- Payment statistics
                    COUNT(DISTINCT pay.Id) AS TotalPayments,
                    COUNT(DISTINCT CASE WHEN pay.Status = 'Paid' THEN pay.Id END) AS PaidPayments,
                    COUNT(DISTINCT CASE WHEN pay.Status = 'Pending' THEN pay.Id END) AS PendingPayments,
                    COUNT(DISTINCT CASE WHEN pay.Status = 'Overdue' THEN pay.Id END) AS OverduePayments,
                    ISNULL(SUM(CASE WHEN pay.Status = 'Paid' THEN pay.Amount ELSE 0 END), 0) AS TotalPaid,
                    ISNULL(SUM(CASE WHEN pay.Status = 'Pending' THEN pay.Amount ELSE 0 END), 0) AS TotalPending,
                    ISNULL(SUM(CASE WHEN pay.Status = 'Overdue' THEN pay.Amount ELSE 0 END), 0) AS TotalOverdue,
                    MAX(CASE WHEN pay.Status = 'Paid' THEN pay.PaidDate END) AS LastPaymentDate,
                    MIN(CASE WHEN pay.Status IN ('Pending', 'Overdue') THEN pay.DueDate END) AS NextPaymentDue,
                    rr.CreatedAt,
                    rr.UpdatedAt
                FROM RentalRecords rr
                INNER JOIN Properties p ON rr.PropertyId = p.Id
                INNER JOIN Users t ON rr.TenantId = t.Id
                LEFT JOIN Users o ON p.CreatedById = o.Id
                LEFT JOIN Payments pay ON rr.Id = pay.RentalId
                GROUP BY 
                    rr.Id, rr.Status, rr.StartDate, rr.EndDate, rr.MonthlyRent, 
                    rr.SecurityDeposit, p.Id, p.Title, p.Address, p.City, p.State,
                    t.Id, t.Username, t.FullName, t.Email, t.Phone,
                    o.Id, o.Username, o.FullName, rr.CreatedAt, rr.UpdatedAt;
            ");

            // Create PaymentSummaryView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_PaymentSummary AS
                SELECT 
                    pay.Id AS PaymentId,
                    pay.Amount,
                    pay.Status AS PaymentStatus,
                    pay.DueDate,
                    pay.PaidDate,
                    pay.PaymentMethod,
                    -- Rental info
                    rr.Id AS RentalId,
                    rr.Status AS RentalStatus,
                    -- Property info
                    p.Id AS PropertyId,
                    p.Title AS PropertyTitle,
                    p.Address AS PropertyAddress,
                    -- Tenant info
                    t.Id AS TenantId,
                    t.FullName AS TenantName,
                    t.Email AS TenantEmail,
                    -- Owner info
                    o.Id AS OwnerId,
                    o.FullName AS OwnerName,
                    -- Calculated fields
                    CASE 
                        WHEN pay.Status = 'Overdue' THEN DATEDIFF(DAY, pay.DueDate, GETUTCDATE())
                        ELSE 0 
                    END AS DaysOverdue,
                    CASE 
                        WHEN pay.Status = 'Pending' AND pay.DueDate > GETUTCDATE() THEN DATEDIFF(DAY, GETUTCDATE(), pay.DueDate)
                        ELSE 0 
                    END AS DaysUntilDue,
                    pay.CreatedAt
                FROM Payments pay
                INNER JOIN RentalRecords rr ON pay.RentalId = rr.Id
                INNER JOIN Properties p ON rr.PropertyId = p.Id
                INNER JOIN Users t ON rr.TenantId = t.Id
                LEFT JOIN Users o ON p.CreatedById = o.Id;
            ");

            // Create InquirySummaryView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_InquirySummary AS
                SELECT 
                    ri.Id AS InquiryId,
                    ri.Name AS InquirerName,
                    ri.Email AS InquirerEmail,
                    ri.Phone AS InquirerPhone,
                    ri.Message,
                    ri.Status AS InquiryStatus,
                    ri.CreatedAt AS InquiryDate,
                    -- Property info
                    p.Id AS PropertyId,
                    p.Title AS PropertyTitle,
                    p.Address AS PropertyAddress,
                    p.City,
                    p.MonthlyRent,
                    p.Status AS PropertyStatus,
                    -- Owner info
                    o.Id AS OwnerId,
                    o.FullName AS OwnerName,
                    o.Email AS OwnerEmail,
                    -- Rental info (if converted)
                    rr.Id AS RentalRecordId,
                    rr.Status AS RentalStatus,
                    rr.StartDate AS RentalStartDate,
                    -- Response time
                    DATEDIFF(HOUR, ri.CreatedAt, GETUTCDATE()) AS HoursSinceInquiry
                FROM RentalInquiries ri
                LEFT JOIN Properties p ON ri.PropertyId = p.Id
                LEFT JOIN Users o ON p.CreatedById = o.Id
                LEFT JOIN RentalRecords rr ON ri.RentalRecordId = rr.Id;
            ");

            // Create ActivitySummaryView
            migrationBuilder.Sql(@"
                CREATE VIEW vw_ActivitySummary AS
                SELECT 
                    a.Id AS ActivityId,
                    a.Type AS ActivityType,
                    a.Description,
                    a.CreatedAt AS ActivityDate,
                    -- User info
                    u.Id AS UserId,
                    u.Username,
                    u.FullName,
                    u.Role,
                    -- Time grouping
                    CAST(a.CreatedAt AS DATE) AS ActivityDay,
                    DATEPART(YEAR, a.CreatedAt) AS ActivityYear,
                    DATEPART(MONTH, a.CreatedAt) AS ActivityMonth,
                    DATEPART(WEEK, a.CreatedAt) AS ActivityWeek
                FROM Activities a
                INNER JOIN Users u ON a.UserId = u.Id;
            ");

            // Update admin user timestamp
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 15, 46, 0, 0, DateTimeKind.Utc), "$2a$11$NewHashForMigration123456789012345678901234567890123456" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop views
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ActivitySummary;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_InquirySummary;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_PaymentSummary;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_RentalDetails;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_PropertyDetails;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserOwnership;");

            // Drop tables
            migrationBuilder.Sql("DROP TABLE IF EXISTS RentalStatistics;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS PropertyStatistics;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS UserOwnershipStats;");

            // Revert admin user
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 9, 49, 4, 322, DateTimeKind.Utc).AddTicks(1018), "$2a$11$R47OK1Oag/GZkOXb2qPkfO6PJB4Kcy8DDuU6MfDWt1iRq1/hRnDTu" });
        }
    }
}
