using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PercentagesAddedToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyPartFocus",
                table: "TrainingSets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, float>>(
                name: "BodyPartPercentages",
                table: "TrainingSets",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, float>>(
                name: "MusclePercentages",
                table: "TrainingSets",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingType",
                table: "TrainingSets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyPartFocus",
                table: "TrainingSets");

            migrationBuilder.DropColumn(
                name: "BodyPartPercentages",
                table: "TrainingSets");

            migrationBuilder.DropColumn(
                name: "MusclePercentages",
                table: "TrainingSets");

            migrationBuilder.DropColumn(
                name: "TrainingType",
                table: "TrainingSets");
        }
    }
}
