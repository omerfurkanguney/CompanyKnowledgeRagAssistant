using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyKnowledgeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentStorageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "documents",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoragePath",
                table: "documents",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "documents",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "documents",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "StoragePath",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "documents");
        }
    }
}
