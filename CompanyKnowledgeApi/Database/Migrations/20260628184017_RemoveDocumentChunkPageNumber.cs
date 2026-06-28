using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyKnowledgeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDocumentChunkPageNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "document_chunks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "document_chunks",
                type: "integer",
                nullable: true);
        }
    }
}
