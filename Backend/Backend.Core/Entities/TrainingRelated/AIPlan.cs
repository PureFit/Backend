using Backend.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core.Entities.TrainingRelated
{
    public class AIPlan
    {
        public Guid Id { get; set; }
        public PlanStatus Status { get; set; }
        public int WeeksDuration { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public PlanType PlanType { get; set; }
        public PlanSubType PlanSubType { get; set; }

        [Column(TypeName = "jsonb")]
        public string? GoalMetadata { get; set; }

        public int SessionsPerWeek { get; set; }
        public int SessionDurationMinutes { get; set; }
        public int PlanDurationWeeks { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> AvailableEquipment { get; set; } = [];

        public string? FreeTextWish { get; set; }

        public Guid? CurrentWeekId { get; set; }
        public WeekPlan? CurrentWeek { get; set; }

        public List<WeekPlan> WeekPlans { get; set; } = [];

        public UserInfo UserInfo { get; set; } = null!;
        public Guid UserInfoId { get; set; }
    }
}
