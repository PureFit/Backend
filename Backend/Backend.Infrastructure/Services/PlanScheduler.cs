using Backend.Application.DTOs.Plan;
using Backend.Application.DTOs.Profile;
using Backend.Application.Repositories;
using Backend.Application.Services;
using Backend.Infrastructure.Helpers;

namespace Backend.Infrastructure.Services;

public class PlanScheduler : IPlanScheduler
{
    private readonly IProfileService _profileService;
    private readonly IUserGoogleTokenRepository _googleTokenRepository;

    public PlanScheduler(IProfileService profileService, IUserGoogleTokenRepository googleTokenRepository)
    {
        _profileService = profileService;
        _googleTokenRepository = googleTokenRepository;
    }

    public async Task<ScheduledPlanDto> ScheduleAsync(PlanFullDto plan, Guid userId, int sessionDurationMinutes)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today);

        List<GoogleCalendarEventDto> calendarEvents = [];

        var googleToken = await _googleTokenRepository.GetByUserIdAsync(userId);
        if (googleToken != null && googleToken.IsActive)
        {
            var planEndDate = startDate.ToDateTime(TimeOnly.MinValue).AddDays(plan.Weeks.Count * 7);
            var eventsResult = await _profileService.GetGoogleCalendarEventsAsync(
                userId, DateTime.Today, planEndDate);

            if (eventsResult.Success && eventsResult.Data != null)
                calendarEvents = eventsResult.Data;
        }

        var scheduledWeeks = new List<ScheduledWeekDto>();

        foreach (var week in plan.Weeks)
        {
            var weekStart = startDate.AddDays((week.WeekNumber - 1) * 7);
            var weekEnd = weekStart.AddDays(6);

            var trainingSlots = TrainingSlotHelper
                .FindTrainingSlots(weekStart, week.Trainings.Count, sessionDurationMinutes, calendarEvents)
                .OrderBy(d => d)
                .ToList();

            var sortedTrainings = week.Trainings.OrderBy(t => t.TrainingNumber).ToList();

            var scheduledTrainings = sortedTrainings
                .Select((t, i) => new ScheduledTrainingDto
                {
                    TrainingNumber = t.TrainingNumber,
                    Name = t.Name,
                    Description = t.Description,
                    EstimatedDurationMinutes = t.EstimatedDurationMinutes,
                    StartPlannedDate = trainingSlots[i],
                    Blocks = t.Blocks
                })
                .ToList();

            scheduledWeeks.Add(new ScheduledWeekDto
            {
                WeekNumber = week.WeekNumber,
                Description = week.Description,
                StartDate = weekStart,
                EndDate = weekEnd,
                Trainings = scheduledTrainings
            });
        }

        return new ScheduledPlanDto
        {
            Name = plan.Name,
            Description = plan.Description,
            Weeks = scheduledWeeks
        };
    }
}
