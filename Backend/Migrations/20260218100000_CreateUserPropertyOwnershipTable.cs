using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniRent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserPropertyOwnershipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop table if it exists (in case of partial creation)
            migrationBuilder.Sql(@"
                IF OBJECT_ID('UserPropertyOwnership', 'U') IS NOT NULL
                    DROP TABLE UserPropertyOwnership;
            ");

            // Create UserPropertyOwnership table
            migrationBuilder.CreateTable(
                name: "UserPropertyOwnership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnershipStartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyOwnership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPropertyOwnership_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPropertyOwnership_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnership_UserId",
                table: "UserPropertyOwnership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnership_PropertyId",
                table: "UserPropertyOwnership",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnership_IsActive",
                table: "UserPropertyOwnership",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyOwnership_UserId_PropertyId",
                table: "UserPropertyOwnership",
                columns: new[] { "UserId", "PropertyId" },
                unique: true);

            // Update admin user timestamp
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 18, 10, 0, 0, 0, DateTimeKind.Utc), "$2a$11$TableCreationMigration12345678901234567890123456789012" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPropertyOwnership");

            // Revert admin user
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 16, 0, 0, 0, DateTimeKind.Utc), "$2a$11$UpdatedHashForOwnershipMigration1234567890123456789012" });
        }
    }
}
