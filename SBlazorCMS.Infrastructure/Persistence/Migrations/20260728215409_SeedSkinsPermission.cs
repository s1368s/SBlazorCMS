using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBlazorCMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSkinsPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { new Guid("55555555-5555-5555-5555-000000000010"), "skins.manage", "مدیریت قالب‌ها" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { new Guid("55555555-5555-5555-5555-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("55555555-5555-5555-5555-000000000010"), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-000000000010"));
        }
    }
}
