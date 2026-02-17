using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniRent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnershipStatsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 12, 46, 21, 834, DateTimeKind.Utc).AddTicks(6326), "$2a$11$z8mmNnu3QJhc7wGKWkGJhuMz0wfht.5wfIGaX496xkzCMtHhMnGTm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 17, 9, 49, 4, 322, DateTimeKind.Utc).AddTicks(1018), "$2a$11$R47OK1Oag/GZkOXb2qPkfO6PJB4Kcy8DDuU6MfDWt1iRq1/hRnDTu" });
        }
    }
}
