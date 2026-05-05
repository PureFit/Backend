using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlTrainingSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_WeekPlans_WeekPlanId",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_WeekPlanId",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "WeekPlanId",
                table: "TrainingSessions");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TrainingSets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TrainingSets");

            migrationBuilder.AddColumn<Guid>(
                name: "WeekPlanId",
                table: "TrainingSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_WeekPlanId",
                table: "TrainingSessions",
                column: "WeekPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_WeekPlans_WeekPlanId",
                table: "TrainingSessions",
                column: "WeekPlanId",
                principalTable: "WeekPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
