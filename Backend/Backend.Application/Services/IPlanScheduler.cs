using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IPlanScheduler
{
    Task<ScheduledPlanDto> ScheduleAsync(PlanFullDto plan, Guid userId, int sessionDurationMinutes);
}
