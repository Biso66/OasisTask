using Microsoft.EntityFrameworkCore.Migrations;
using Oasis.Helpers;

#nullable disable

namespace Oasis.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class SeedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.InsertData(
                     table: "AspNetRoles",
                     columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), Roles.Admin, Roles.Admin.ToUpper(), Guid.NewGuid().ToString() });
                migrationBuilder.InsertData(
                table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), Roles.User, Roles.User.ToUpper(), Guid.NewGuid().ToString() });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
              migrationBuilder.Sql("DELETE FROM [AspNetRoles]");

        }
    }
}
