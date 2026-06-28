using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyKnowledgeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChatSessionSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "chat_sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "chat_sessions");
        }
    }
}
