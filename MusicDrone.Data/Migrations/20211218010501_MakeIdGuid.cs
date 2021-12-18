using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicDrone.Data.Migrations
{
    public partial class MakeIdGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "330efa8c-cea1-4952-889d-df93cea5246c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "986d6f5b-4ecc-483b-8922-dd56033d0a10");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd2cadc0-c19a-4c6a-b611-91cbb6e8003e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d4f51cec-37cf-4b82-a3cc-30c9549fa88f", "5b010cdb-f028-40fd-b2d8-d1d5c1af4f58" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4f51cec-37cf-4b82-a3cc-30c9549fa88f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5b010cdb-f028-40fd-b2d8-d1d5c1af4f58");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "RoomsUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "RoomsUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RoomsUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c12bc81e-c709-466a-b5c0-b871e6a22695", "623be92e-0240-4ad1-8ae0-fe5b789d66b2", "Users", "USERS" },
                    { "41ae5496-f16b-4767-8815-fa14d80069e8", "118e8680-d2c2-4f10-9209-3ff75fb0e8cd", "PremiumUsers", "PREMIUMUSERS" },
                    { "0b8792fc-8455-4eba-8a03-186a444294df", "2ae7bd71-19bd-4596-a0bc-ef1828d1d2c7", "Moderators", "MODERATORS" },
                    { "a5810127-d7fd-452c-9508-f26634640999", "fcd17cba-cb13-420a-a510-7a0b62eaeb8b", "Administrators", "ADMINISTRATORS" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d5357248-d101-4414-b416-7a236e7ee713", 0, "c67eb37d-8aa1-493f-aef2-2c446e6d3ce1", "admin@music.drone", false, "AdminName", "AdminLastName", false, null, "ADMIN@MUSIC.DRONE", "ADMIN@MUSIC.DRONE", "AQAAAAEAACcQAAAAEK8rz4AnE8z8Tqxm1VDvp9uxtN4MGu+d9wrmTgr1WS5e+jdx5WNGojFjgBaFf2D/Fg==", null, false, "37e13681-5bab-4a04-9bc6-6f6c69f5a1d5", false, "admin@music.drone" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a5810127-d7fd-452c-9508-f26634640999", "d5357248-d101-4414-b416-7a236e7ee713" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b8792fc-8455-4eba-8a03-186a444294df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41ae5496-f16b-4767-8815-fa14d80069e8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c12bc81e-c709-466a-b5c0-b871e6a22695");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a5810127-d7fd-452c-9508-f26634640999", "d5357248-d101-4414-b416-7a236e7ee713" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5810127-d7fd-452c-9508-f26634640999");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d5357248-d101-4414-b416-7a236e7ee713");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RoomsUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "RoomsUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RoomsUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "330efa8c-cea1-4952-889d-df93cea5246c", "64a4d278-5ba6-42fb-9e29-877e9dda7639", "Users", "USERS" },
                    { "986d6f5b-4ecc-483b-8922-dd56033d0a10", "794ca157-1a03-4ee4-93f6-c340ee63e800", "PremiumUsers", "PREMIUMUSERS" },
                    { "cd2cadc0-c19a-4c6a-b611-91cbb6e8003e", "aa30a880-aff6-4e9f-a8e3-9bf7b88cc368", "Moderators", "MODERATORS" },
                    { "d4f51cec-37cf-4b82-a3cc-30c9549fa88f", "f5ca7c1c-dd2d-495c-9431-c706ece4f023", "Administrators", "ADMINISTRATORS" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5b010cdb-f028-40fd-b2d8-d1d5c1af4f58", 0, "a67e4404-cefb-4cae-83d7-9207dbacc55d", "admin@music.drone", false, "AdminName", "AdminLastName", false, null, "ADMIN@MUSIC.DRONE", "ADMIN@MUSIC.DRONE", "AQAAAAEAACcQAAAAEA0C+lc71hQbpGId3CtTR1GaPYYD/X0abKygw+KJcY1HQs+1fP3JSwmXiHtjb73J5w==", null, false, "296221cf-63d6-4115-9c38-b591c85a0f16", false, "admin@music.drone" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d4f51cec-37cf-4b82-a3cc-30c9549fa88f", "5b010cdb-f028-40fd-b2d8-d1d5c1af4f58" });
        }
    }
}
