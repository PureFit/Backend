using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyParts",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyParts", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ExerciseId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    ImageUrl360p = table.Column<string>(type: "text", nullable: true),
                    ImageUrl480p = table.Column<string>(type: "text", nullable: true),
                    ImageUrl720p = table.Column<string>(type: "text", nullable: true),
                    ImageUrl1080p = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: false),
                    ExerciseType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BodyParts = table.Column<List<string>>(type: "jsonb", nullable: false),
                    TargetMuscles = table.Column<List<string>>(type: "jsonb", nullable: false),
                    SecondaryMuscles = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Equipments = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Keywords = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Instructions = table.Column<List<string>>(type: "jsonb", nullable: false),
                    ExerciseTips = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Variations = table.Column<List<string>>(type: "jsonb", nullable: false),
                    RelatedExerciseIds = table.Column<List<string>>(type: "jsonb", nullable: false),
                    CachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ExerciseId);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyParts");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "ExerciseTypes");

            migrationBuilder.DropTable(
                name: "Muscles");
        }
    }
}
