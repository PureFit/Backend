using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    // Тренировка вне нашего приложения (дзюдо, зал без приложения и т.д.).
    // Нужна для оценки общей нагрузки пользователя и адаптации AI-плана.
    // Может быть добавлена вручную или привязана к событию Google Calendar.
    public class ExternalTraining
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;        // "Тренировка по дзюдо", "Зал"
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public int? DurationMinutes { get; set; }

        // Субъективная оценка нагрузки от пользователя.
        public PerceivedExertion? PerceivedExertion { get; set; }

        // Расчётная нагрузка = (int)PerceivedExertion * DurationMinutes.
        // Используется AI для оценки общей недельной нагрузки и адаптации плана.
        public int? LoadScore { get; set; }

        // Если привязана к событию Google Calendar — храним ID события.
        // Позволяет избежать дублирования при синхронизации.
        public string? GoogleEventId { get; set; }

        // Основная привязка — всегда к пользователю.
        public Guid UserInfoId { get; set; }
        public UserInfo UserInfo { get; set; } = null!;

        // Опционально — привязка к неделе плана если план есть.
        public Guid? WeekPlanId { get; set; }
        public WeekPlan? WeekPlan { get; set; }
    }
}
