using Backend.Core.Entities.ExerciseRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseBodyPartConfiguration : IEntityTypeConfiguration<ExerciseBodyPart>
{
    public void Configure(EntityTypeBuilder<ExerciseBodyPart> builder)
    {
        builder.HasKey(eb => new { eb.ExerciseId, eb.BodyPartId });
    }
}
