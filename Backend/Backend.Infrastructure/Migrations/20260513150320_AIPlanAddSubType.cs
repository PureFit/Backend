using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AIPlanAddSubType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "AvailableEquipment",
                table: "AiPlans",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "FreeTextWish",
                table: "AiPlans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanDurationWeeks",
                table: "AiPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlanSubType",
                table: "AiPlans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SessionDurationMinutes",
                table: "AiPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SessionsPerWeek",
                table: "AiPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableEquipment",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "FreeTextWish",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "PlanDurationWeeks",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "PlanSubType",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "SessionDurationMinutes",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "SessionsPerWeek",
                table: "AiPlans");
        }
    }
}
