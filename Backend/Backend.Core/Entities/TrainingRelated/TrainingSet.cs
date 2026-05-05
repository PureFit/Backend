using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    public class TrainingSet
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public SetAccessType SetAccessType { get; set; }

        public List<SetBlock> SetBlocks { get; set; } = [];

        public PlanTraining? PlanTraining { get; set; }
        public Guid? PlanTrainingId { get; set; }
    }
}
