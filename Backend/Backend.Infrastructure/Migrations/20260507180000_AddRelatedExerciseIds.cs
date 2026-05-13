using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatedExerciseIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Exercises\" ADD COLUMN IF NOT EXISTS \"RelatedExerciseIds\" jsonb;");
            migrationBuilder.Sql("UPDATE \"Exercises\" SET \"RelatedExerciseIds\" = '[]'::jsonb WHERE \"RelatedExerciseIds\" IS NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Exercises\" ALTER COLUMN \"RelatedExerciseIds\" SET NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE \"Exercises\" ALTER COLUMN \"RelatedExerciseIds\" SET DEFAULT '[]';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedExerciseIds",
                table: "Exercises");
        }
    }
}
