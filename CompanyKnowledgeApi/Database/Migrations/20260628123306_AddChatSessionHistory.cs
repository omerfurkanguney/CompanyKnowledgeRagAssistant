using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyKnowledgeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChatSessionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_sessions_users_UserId",
                table: "chat_sessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "chat_sessions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "SourcesJson",
                table: "chat_messages",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_sessions_users_UserId",
                table: "chat_sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_sessions_users_UserId",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "SourcesJson",
                table: "chat_messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "chat_sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_sessions_users_UserId",
                table: "chat_sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
