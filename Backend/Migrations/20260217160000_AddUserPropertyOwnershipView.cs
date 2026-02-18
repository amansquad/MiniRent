using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniRent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPropertyOwnershipView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create UserPropertyOwnership table for materialized ownership data
            migrationBuilder.Sql(@"
                CREATE TABLE UserPropertyOwnership (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    UserId UNIQUEIDENTIFIER NOT NULL,
                    PropertyId UNIQUEIDENTIFIER NOT NULL,
                    OwnershipStartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    IsActive BIT NOT NULL DEFAULT 1,
                    Notes NVARCHAR(500) NULL,
                    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    UpdatedAt DATETIME2 NULL,
                    CONSTRAINT FK_UserPropertyOwnership_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_UserPropertyOwnership_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE NO ACTION,
                    CONSTRAINT UQ_UserPropertyOwnership_UserProperty UNIQUE (UserId, PropertyId)
                );
                
                CREATE INDEX IX_UserPropertyOwnership_UserId ON UserPropertyOwnership(UserId);
                CREATE INDEX IX_UserPropertyOwnership_PropertyId ON UserPropertyOwnership(PropertyId);
                CREATE INDEX IX_UserPropertyOwnership_IsActive ON UserPropertyOwnership(IsActive);
            ");

            // Create view for user-property ownership details
            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserPropertyOwnership AS
                SELECT 
                    u.Id AS UserId,
                    u.Username,
                    u.FullName AS OwnerName,
                    u.Email AS OwnerEmail,
                    u.Phone AS OwnerPhone,
                    u.Role AS UserRole,
                    p.Id AS PropertyId,
                    p.Title AS PropertyTitle,
                    p.Address AS PropertyAddress,
                    p.City,
                    p.State,
                    p.Country,
                    p.PropertyType,
                    p.Status AS PropertyStatus,
                    p.MonthlyRent,
                    p.Bedrooms,
                    p.Bathrooms,
                    p.Area,
                    p.Floor,
                    p.CreatedAt AS PropertyCreatedAt,
                    p.IsDeleted,
                    -- Rental information
                    COUNT(DISTINCT rr.Id) AS TotalRentals,
                    COUNT(DISTINCT CASE WHEN rr.Status = 'Active' THEN rr.Id END) AS ActiveRentals,
                    MAX(rr.StartDate) AS LastRentalDate,
                    -- Current tenant info (if rented)
                    (SELECT TOP 1 t.FullName 
                     FROM RentalRecords rr2 
                     INNER JOIN Users t ON rr2.TenantId = t.Id 
                     WHERE rr2.PropertyId = p.Id AND rr2.Status = 'Active'
                     ORDER BY rr2.StartDate DESC) AS CurrentTenantName,
                    (SELECT TOP 1 rr2.StartDate 
                     FROM RentalRecords rr2 
                     WHERE rr2.PropertyId = p.Id AND rr2.Status = 'Active'
                     ORDER BY rr2.StartDate DESC) AS CurrentRentalStartDate
                FROM Users u
                INNER JOIN Properties p ON u.Id = p.CreatedById
                LEFT JOIN RentalRecords rr ON p.Id = rr.PropertyId
                WHERE p.IsDeleted = 0
                GROUP BY 
                    u.Id, u.Username, u.FullName, u.Email, u.Phone, u.Role,
                    p.Id, p.Title, p.Address, p.City, p.State, p.Country,
                    p.PropertyType, p.Status, p.MonthlyRent, p.Bedrooms, 
                    p.Bathrooms, p.Area, p.Floor, p.CreatedAt, p.IsDeleted;
            ");

            // Create simplified view for quick ownership lookup
            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserPropertiesSummary AS
                SELECT 
                    u.Id AS UserId,
                    u.Username,
                    u.FullName AS OwnerName,
                    u.Email,
                    u.Role,
                    COUNT(p.Id) AS TotalProperties,
                    COUNT(CASE WHEN p.Status = 'Available' THEN 1 END) AS AvailableProperties,
                    COUNT(CASE WHEN p.Status = 'Rented' THEN 1 END) AS RentedProperties,
                    COUNT(CASE WHEN p.Status = 'Reserved' THEN 1 END) AS ReservedProperties,
                    COUNT(CASE WHEN p.Status = 'Maintenance' THEN 1 END) AS MaintenanceProperties,
                    ISNULL(SUM(CASE WHEN p.Status = 'Rented' THEN p.MonthlyRent ELSE 0 END), 0) AS TotalMonthlyIncome,
                    MIN(p.CreatedAt) AS FirstPropertyDate,
                    MAX(p.CreatedAt) AS LastPropertyDate,
                    CAST(NULL AS NVARCHAR(MAX)) AS Cities
                FROM Users u
                LEFT JOIN Properties p ON u.Id = p.CreatedById AND p.IsDeleted = 0
                GROUP BY u.Id, u.Username, u.FullName, u.Email, u.Role;
            ");

            // Create view for property ownership history
            migrationBuilder.Sql(@"
                CREATE VIEW vw_PropertyOwnershipHistory AS
                SELECT 
                    p.Id AS PropertyId,
                    p.Title AS PropertyTitle,
                    p.Address,
                    p.City,
                    p.State,
                    p.Status,
                    p.MonthlyRent,
                    u.Id AS CurrentOwnerId,
                    u.Username AS CurrentOwnerUsername,
                    u.FullName AS CurrentOwnerName,
                    u.Email AS CurrentOwnerEmail,
                    u.Phone AS CurrentOwnerPhone,
                    p.CreatedAt AS OwnershipStartDate,
                    p.UpdatedAt AS LastModifiedDate,
                    DATEDIFF(DAY, p.CreatedAt, GETUTCDATE()) AS DaysOwned,
                    -- Property performance
                    (SELECT COUNT(*) FROM RentalRecords WHERE PropertyId = p.Id) AS TotalRentals,
                    (SELECT COUNT(*) FROM RentalInquiries WHERE PropertyId = p.Id) AS TotalInquiries,
                    (SELECT AVG(CAST(Rating AS DECIMAL(3,2))) FROM Reviews WHERE PropertyId = p.Id) AS AverageRating
                FROM Properties p
                INNER JOIN Users u ON p.CreatedById = u.Id
                WHERE p.IsDeleted = 0;
            ");

            // Update admin user timestamp
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 16, 0, 0, 0, DateTimeKind.Utc), "$2a$11$UpdatedHashForOwnershipMigration1234567890123456789012" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop views
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_PropertyOwnershipHistory;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserPropertiesSummary;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserPropertyOwnership;");

            // Drop table
            migrationBuilder.Sql("DROP TABLE IF EXISTS UserPropertyOwnership;");

            // Revert admin user
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 15, 46, 0, 0, DateTimeKind.Utc), "$2a$11$NewHashForMigration123456789012345678901234567890123456" });
        }
    }
}
