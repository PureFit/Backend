using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RevertEmbeddingTo768 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "exercise_embeddings",
                type: "vector(768)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(3072)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "exercise_embeddings",
                type: "vector(3072)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(768)");
        }
    }
}
