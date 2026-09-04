using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseEmbeddingConfiguration : IEntityTypeConfiguration<ExerciseEmbeddingRecord>
{
    public void Configure(EntityTypeBuilder<ExerciseEmbeddingRecord> builder)
    {
        builder.ToTable("exercise_embeddings");
        builder.HasKey(e => e.ExerciseId);
        builder.Property(e => e.Embedding).HasColumnType("vector(768)");
    }
}
