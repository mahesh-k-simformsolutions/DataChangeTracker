using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataChangeTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Column_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "Audits",
                newName: "TableName");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Audits",
                newName: "RecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "Audits",
                newName: "EntityName");

            migrationBuilder.RenameColumn(
                name: "RecordId",
                table: "Audits",
                newName: "EntityId");
        }
    }
}
