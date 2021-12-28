using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicDrone.Data.Migrations
{
    public partial class SnapshotSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomsUsers",
                table: "RoomsUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomsUsers",
                table: "RoomsUsers",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d0900cc-5def-4708-b88c-9dd66c7db1ef"),
                column: "ConcurrencyStamp",
                value: "424012bb-cbb1-49a6-a2c7-70f0dbd1348e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("85b19000-bc92-435a-8d6b-ea780217d030"),
                column: "ConcurrencyStamp",
                value: "8badb56b-5e22-4e78-ab3b-7e18477e25c5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bc963479-34f3-42b7-8d32-41bae9c47742"),
                column: "ConcurrencyStamp",
                value: "2d82515d-d9c5-448b-8b14-c89fdc8d8dbf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e3e7bd47-14c6-4a22-81c7-9381d1b4db70"),
                column: "ConcurrencyStamp",
                value: "03fd4a3f-9c7c-477e-b8c5-198f398fdcd0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cdd4f090-d8aa-4c14-9cd5-fe3464ff3bbb"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bd6860b2-2dbd-420f-93de-f7dc8249cddc", "AQAAAAEAACcQAAAAEI2+pzubHtmSDWn0CopjwzlEStiQrTb+B6y7gsx77ztVVvDhyTiZzkbxuUKo8rSJKg==" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomsUsers_UserId",
                table: "RoomsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsUsers_AspNetUsers_UserId",
                table: "RoomsUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsUsers_Rooms_RoomId",
                table: "RoomsUsers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomsUsers_AspNetUsers_UserId",
                table: "RoomsUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomsUsers_Rooms_RoomId",
                table: "RoomsUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomsUsers",
                table: "RoomsUsers");

            migrationBuilder.DropIndex(
                name: "IX_RoomsUsers_UserId",
                table: "RoomsUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomsUsers",
                table: "RoomsUsers",
                column: "Id");

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
    }
}
