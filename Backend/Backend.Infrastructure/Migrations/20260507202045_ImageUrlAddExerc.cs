using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlAddExerc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyPartPercentages",
                table: "TrainingSets");

            migrationBuilder.DropColumn(
                name: "MusclePercentages",
                table: "TrainingSets");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Exercises",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Exercises");

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
        }
    }
}
