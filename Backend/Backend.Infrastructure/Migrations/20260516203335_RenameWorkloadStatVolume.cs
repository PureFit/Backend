using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkloadStatVolume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPercent",
                table: "UserWorkloadStats",
                newName: "AccumulatedVolume");

            migrationBuilder.AddColumn<int>(
                name: "SessionCount",
                table: "UserWorkloadStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionCount",
                table: "UserWorkloadStats");

            migrationBuilder.RenameColumn(
                name: "AccumulatedVolume",
                table: "UserWorkloadStats",
                newName: "TotalPercent");
        }
    }
}
