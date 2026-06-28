using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyKnowledgeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentChunkMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChunkType",
                table: "document_chunks",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Fixed");

            migrationBuilder.AddColumn<string>(
                name: "ClauseId",
                table: "document_chunks",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndPageNumber",
                table: "document_chunks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Heading",
                table: "document_chunks",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartPageNumber",
                table: "document_chunks",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "ChunkType",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "ClauseId",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "EndPageNumber",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "Heading",
                table: "document_chunks");

            migrationBuilder.DropColumn(
                name: "StartPageNumber",
                table: "document_chunks");
        }
    }
}
