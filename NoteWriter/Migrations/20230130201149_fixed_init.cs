using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteWriter.Migrations
{
    /// <inheritdoc />
    public partial class fixedinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WDC",
                table: "WDC");

            migrationBuilder.RenameTable(
                name: "WDC",
                newName: "Notes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "WDC");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WDC",
                table: "WDC",
                column: "Id");
        }
    }
}
