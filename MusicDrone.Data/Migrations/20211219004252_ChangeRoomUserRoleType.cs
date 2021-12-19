using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicDrone.Data.Migrations
{
    public partial class ChangeRoomUserRoleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "RoomsUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d0900cc-5def-4708-b88c-9dd66c7db1ef"),
                column: "ConcurrencyStamp",
                value: "ac14b202-3232-4858-84d7-136c78aca7bf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("85b19000-bc92-435a-8d6b-ea780217d030"),
                column: "ConcurrencyStamp",
                value: "46abe5f3-d066-4cd6-b258-daeea78a5e7c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bc963479-34f3-42b7-8d32-41bae9c47742"),
                column: "ConcurrencyStamp",
                value: "0b22593e-5147-4846-a502-a57d782c0340");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e3e7bd47-14c6-4a22-81c7-9381d1b4db70"),
                column: "ConcurrencyStamp",
                value: "a144e69f-ed86-4759-9374-d3083a3b607c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cdd4f090-d8aa-4c14-9cd5-fe3464ff3bbb"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ec01c9cf-b6d7-4efd-bb35-010a04010291", "AQAAAAEAACcQAAAAEHqHbUJwhmtLXWH18gJs/5GYyuxPPlKtXu1AgrHiza+1Tx5yx52jCQpHfbX+Hd1mQg==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "RoomsUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d0900cc-5def-4708-b88c-9dd66c7db1ef"),
                column: "ConcurrencyStamp",
                value: "3949fdf9-4f1e-4691-859b-e7ed47c97b6e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("85b19000-bc92-435a-8d6b-ea780217d030"),
                column: "ConcurrencyStamp",
                value: "1818e045-15f8-4e58-8aa1-1a2f58e7e3d4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bc963479-34f3-42b7-8d32-41bae9c47742"),
                column: "ConcurrencyStamp",
                value: "3d0b3d2d-0e41-4c01-96fa-275ee041d03a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e3e7bd47-14c6-4a22-81c7-9381d1b4db70"),
                column: "ConcurrencyStamp",
                value: "cb915358-d127-4eb7-af2b-35047e08d30e");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cdd4f090-d8aa-4c14-9cd5-fe3464ff3bbb"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4024b053-ee5e-415a-98b4-466868ea6950", "AQAAAAEAACcQAAAAEDve51iL/rXOsZIdpyijvbPcQ9SvPtc1VYWZ1iqPUKK29YVyta78qsGiJ3g0SqOPqg==" });
        }
    }
}
