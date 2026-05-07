using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExternalApiExecriceMusclesFieldsMovedToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressionType",
                table: "ExerciseEntries");

            migrationBuilder.AddColumn<List<string>>(
                name: "BodyParts",
                table: "ExerciseEntries",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "SecondaryMuscles",
                table: "ExerciseEntries",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "TargetMuscles",
                table: "ExerciseEntries",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyParts",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "SecondaryMuscles",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "TargetMuscles",
                table: "ExerciseEntries");

            migrationBuilder.AddColumn<string>(
                name: "ProgressionType",
                table: "ExerciseEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
