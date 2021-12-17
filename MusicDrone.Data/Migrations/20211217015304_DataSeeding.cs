using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicDrone.Data.Migrations
{
    public partial class DataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
