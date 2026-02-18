using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniRent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyImageUrlLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "PropertyImages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "PropertyStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalRentals = table.Column<int>(type: "int", nullable: false),
                    ActiveRentals = table.Column<int>(type: "int", nullable: false),
                    TotalInquiries = table.Column<int>(type: "int", nullable: false),
                    PendingInquiries = table.Column<int>(type: "int", nullable: false),
                    TotalReviews = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OccupancyRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LastRentalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyStatistics_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPayments = table.Column<int>(type: "int", nullable: false),
                    PaidPayments = table.Column<int>(type: "int", nullable: false),
                    PendingPayments = table.Column<int>(type: "int", nullable: false),
                    OverduePayments = table.Column<int>(type: "int", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPending = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOverdue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentCompletionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextPaymentDue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalStatistics_RentalRecords_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOwnershipStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalProperties = table.Column<int>(type: "int", nullable: false),
                    AvailableProperties = table.Column<int>(type: "int", nullable: false),
                    RentedProperties = table.Column<int>(type: "int", nullable: false),
                    ReservedProperties = table.Column<int>(type: "int", nullable: false),
                    MaintenanceProperties = table.Column<int>(type: "int", nullable: false),
                    TotalRentalsAsTenant = table.Column<int>(type: "int", nullable: false),
                    ActiveRentalsAsTenant = table.Column<int>(type: "int", nullable: false),
                    EndedRentalsAsTenant = table.Column<int>(type: "int", nullable: false),
                    TotalMonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOwnershipStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOwnershipStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPropertyOwnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnershipStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPropertyOwnerships_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPropertyOwnerships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 18, 8, 29, 20, 273, DateTimeKind.Utc).AddTicks(2744), "$2a$11$RD8B.NabYPU60tTwhG8PIuqUIYe0BCiSARp.k87JZe9JHmhR20XwO" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyStatistics_PropertyId",
                table: "PropertyStatistics",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalStatistics_RentalId",
                table: "RentalStatistics",
                column: "RentalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnershipStats_UserId",
                table: "UserOwnershipStats",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnerships_IsActive",
                table: "UserPropertyOwnerships",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnerships_PropertyId",
                table: "UserPropertyOwnerships",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnerships_UserId",
                table: "UserPropertyOwnerships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnerships_UserId_PropertyId",
                table: "UserPropertyOwnerships",
                columns: new[] { "UserId", "PropertyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyStatistics");

            migrationBuilder.DropTable(
                name: "RentalStatistics");

            migrationBuilder.DropTable(
                name: "UserOwnershipStats");

            migrationBuilder.DropTable(
                name: "UserPropertyOwnerships");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "PropertyImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 12, 46, 21, 834, DateTimeKind.Utc).AddTicks(6326), "$2a$11$z8mmNnu3QJhc7wGKWkGJhuMz0wfht.5wfIGaX496xkzCMtHhMnGTm" });
        }
    }
}
