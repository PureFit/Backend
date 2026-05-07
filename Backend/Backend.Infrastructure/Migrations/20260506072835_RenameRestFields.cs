using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RestTimeAfterBlockDoneSeconds",
                table: "SetBlocks",
                newName: "RestBetweenSetsSeconds");

            migrationBuilder.AddColumn<int>(
                name: "RestAfterBlockSeconds",
                table: "SetBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestAfterBlockSeconds",
                table: "SetBlocks");

            migrationBuilder.RenameColumn(
                name: "RestBetweenSetsSeconds",
                table: "SetBlocks",
                newName: "RestTimeAfterBlockDoneSeconds");
        }
    }
}
