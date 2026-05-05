using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoreTrainingsLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "UserInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentPlanId",
                table: "UserInfos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "UserInfos",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "UserWorkloadStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    TotalPercent = table.Column<float>(type: "real", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkloadStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkloadStats_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    WeeksDuration = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlanType = table.Column<string>(type: "text", nullable: false),
                    GoalMetadata = table.Column<string>(type: "jsonb", nullable: true),
                    CurrentWeekId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiPlans_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumberInPlan = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WeekStatus = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AIPlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekPlans_AiPlans_AIPlanId",
                        column: x => x.AIPlanId,
                        principalTable: "AiPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    PerceivedExertion = table.Column<string>(type: "text", nullable: true),
                    LoadScore = table.Column<int>(type: "integer", nullable: true),
                    GoogleEventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekPlanId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalTrainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalTrainings_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalTrainings_WeekPlans_WeekPlanId",
                        column: x => x.WeekPlanId,
                        principalTable: "WeekPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlanTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StartPlannedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndPlannedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WeekPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingSetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTrainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanTrainings_WeekPlans_WeekPlanId",
                        column: x => x.WeekPlanId,
                        principalTable: "WeekPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetAccessType = table.Column<string>(type: "text", nullable: false),
                    PlanTrainingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSets_PlanTrainings_PlanTrainingId",
                        column: x => x.PlanTrainingId,
                        principalTable: "PlanTrainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingSets_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SetsCount = table.Column<int>(type: "integer", nullable: false),
                    RestTimeAfterBlockDoneSeconds = table.Column<int>(type: "integer", nullable: false),
                    TrainingSetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetBlocks_TrainingSets_TrainingSetId",
                        column: x => x.TrainingSetId,
                        principalTable: "TrainingSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    PlanTrainingId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrainingSetId = table.Column<Guid>(type: "uuid", nullable: true),
                    WeekPlanId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_PlanTrainings_PlanTrainingId",
                        column: x => x.PlanTrainingId,
                        principalTable: "PlanTrainings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingSessions_TrainingSets_TrainingSetId",
                        column: x => x.TrainingSetId,
                        principalTable: "TrainingSets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingSessions_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_WeekPlans_WeekPlanId",
                        column: x => x.WeekPlanId,
                        principalTable: "WeekPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MeasureType = table.Column<string>(type: "text", nullable: false),
                    ProgressionType = table.Column<string>(type: "text", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    Parameters = table.Column<Dictionary<string, float>>(type: "jsonb", nullable: true),
                    RestAfterCurrentEntrySeconds = table.Column<int>(type: "integer", nullable: false),
                    SetBlockId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseEntries_SetBlocks_SetBlockId",
                        column: x => x.SetBlockId,
                        principalTable: "SetBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseIntervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    Parameters = table.Column<Dictionary<string, float>>(type: "jsonb", nullable: true),
                    ExerciseEntryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseIntervals_ExerciseEntries_ExerciseEntryId",
                        column: x => x.ExerciseEntryId,
                        principalTable: "ExerciseEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_CurrentPlanId",
                table: "UserInfos",
                column: "CurrentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AiPlans_CurrentWeekId",
                table: "AiPlans",
                column: "CurrentWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_AiPlans_UserInfoId",
                table: "AiPlans",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseEntries_SetBlockId",
                table: "ExerciseEntries",
                column: "SetBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseIntervals_ExerciseEntryId",
                table: "ExerciseIntervals",
                column: "ExerciseEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalTrainings_UserInfoId",
                table: "ExternalTrainings",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalTrainings_WeekPlanId",
                table: "ExternalTrainings",
                column: "WeekPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanTrainings_WeekPlanId",
                table: "PlanTrainings",
                column: "WeekPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SetBlocks_TrainingSetId",
                table: "SetBlocks",
                column: "TrainingSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_PlanTrainingId",
                table: "TrainingSessions",
                column: "PlanTrainingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TrainingSetId",
                table: "TrainingSessions",
                column: "TrainingSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_UserInfoId",
                table: "TrainingSessions",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_WeekPlanId",
                table: "TrainingSessions",
                column: "WeekPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSets_CreatedByUserId",
                table: "TrainingSets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSets_PlanTrainingId",
                table: "TrainingSets",
                column: "PlanTrainingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkloadStats_UserInfoId_Name_Category",
                table: "UserWorkloadStats",
                columns: new[] { "UserInfoId", "Name", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeekPlans_AIPlanId",
                table: "WeekPlans",
                column: "AIPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_AiPlans_CurrentPlanId",
                table: "UserInfos",
                column: "CurrentPlanId",
                principalTable: "AiPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AiPlans_WeekPlans_CurrentWeekId",
                table: "AiPlans",
                column: "CurrentWeekId",
                principalTable: "WeekPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_AiPlans_CurrentPlanId",
                table: "UserInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_AiPlans_WeekPlans_CurrentWeekId",
                table: "AiPlans");

            migrationBuilder.DropTable(
                name: "ExerciseIntervals");

            migrationBuilder.DropTable(
                name: "ExternalTrainings");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "UserWorkloadStats");

            migrationBuilder.DropTable(
                name: "ExerciseEntries");

            migrationBuilder.DropTable(
                name: "SetBlocks");

            migrationBuilder.DropTable(
                name: "TrainingSets");

            migrationBuilder.DropTable(
                name: "PlanTrainings");

            migrationBuilder.DropTable(
                name: "WeekPlans");

            migrationBuilder.DropTable(
                name: "AiPlans");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_CurrentPlanId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "CurrentPlanId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "UserInfos");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "UserInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
