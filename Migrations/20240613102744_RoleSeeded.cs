using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthenticationAPI.Migrations
{
    /// <inheritdoc />
    public partial class RoleSeeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a3f3bc6f-ff73-455b-9cec-64b181bcdcee", "2", "HR", "HR" },
                    { "a7667fb2-853f-47e5-83ba-4822dc5a9914", "1", "Admin", "Admin" },
                    { "a9b989b4-6cff-4bbf-b8db-c0b9b886bc51", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a3f3bc6f-ff73-455b-9cec-64b181bcdcee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7667fb2-853f-47e5-83ba-4822dc5a9914");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a9b989b4-6cff-4bbf-b8db-c0b9b886bc51");
        }
    }
}
