using Backend.Application.DTOs.Profile;

namespace Backend.Infrastructure.Helpers;

internal static class TrainingSlotHelper
{
    private static readonly TimeOnly WorkdayStart = new(7, 0);
    private static readonly TimeOnly WorkdayEnd = new(22, 0);

    public static List<DateTime> FindTrainingSlots(
        DateOnly weekStart,
        int count,
        int durationMinutes,
        List<GoogleCalendarEventDto> calendarEvents)
    {
        var idealOffsets = GetIdealOffsets(count);
        var result = new List<DateTime>();
        var usedDays = new HashSet<DateOnly>();

        // Уровень 1: идеальные дни (максимальный разброс)
        foreach (var offset in idealOffsets)
        {
            if (result.Count >= count) break;
            var day = weekStart.AddDays(offset);
            var slot = FindFreeSlotInDay(day, durationMinutes, calendarEvents);
            if (slot.HasValue)
            {
                result.Add(slot.Value);
                usedDays.Add(day);
            }
        }

        // Уровень 2: оставшиеся дни — приоритет тем, кто дальше от уже занятых
        if (result.Count < count)
        {
            var candidates = Enumerable.Range(0, 7)
                .Select(i => weekStart.AddDays(i))
                .Where(d => !usedDays.Contains(d))
                .OrderByDescending(d => MinDistanceFromUsed(d, usedDays))
                .ToList();

            foreach (var day in candidates)
            {
                if (result.Count >= count) break;
                var slot = FindFreeSlotInDay(day, durationMinutes, calendarEvents);
                if (slot.HasValue)
                {
                    result.Add(slot.Value);
                    usedDays.Add(day);
                }
            }
        }

        // Уровень 3: крайний случай — 9:00 без проверки, max-gap порядок
        if (result.Count < count)
        {
            var candidates = Enumerable.Range(0, 7)
                .Select(i => weekStart.AddDays(i))
                .Where(d => !usedDays.Contains(d))
                .OrderByDescending(d => MinDistanceFromUsed(d, usedDays))
                .ToList();

            foreach (var day in candidates)
            {
                if (result.Count >= count) break;
                result.Add(DateTime.SpecifyKind(day.ToDateTime(new TimeOnly(9, 0)), DateTimeKind.Utc));
                usedDays.Add(day);
            }
        }

        return result.OrderBy(d => d).Take(count).ToList();
    }

    public static DateTime? FindFreeSlotInDay(
        DateOnly day,
        int durationMinutes,
        List<GoogleCalendarEventDto> calendarEvents)
    {
        var eventsOnDay = calendarEvents
            .Where(e => DateOnly.FromDateTime(e.StartTime) == day)
            .ToList();

        var current = WorkdayStart;

        while (current.AddMinutes(durationMinutes) <= WorkdayEnd)
        {
            var slotStart = DateTime.SpecifyKind(day.ToDateTime(current), DateTimeKind.Utc);
            var slotEnd = slotStart.AddMinutes(durationMinutes);

            var hasConflict = eventsOnDay.Any(e =>
                e.StartTime < slotEnd && e.EndTime > slotStart);

            if (!hasConflict)
                return slotStart;

            current = current.AddMinutes(15);
        }

        return null;
    }

    // 1 → середина недели
    // 2 → Вт/Сб (разрыв 4 дня)
    // 3 → Вт/Чт/Сб (разрыв 2 дня)
    // 4 → Пн/Ср/Пт/Вс (через день)
    // 5 → Пн/Вт/Чт/Сб/Вс (отдых Ср)
    // 6 → Пн-Сб (отдых Вс)
    private static int[] GetIdealOffsets(int count) => count switch
    {
        1 => [3],
        2 => [1, 5],
        3 => [1, 3, 5],
        4 => [0, 2, 4, 6],
        5 => [0, 1, 3, 5, 6],
        6 => [0, 1, 2, 3, 4, 5],
        _ => [0, 1, 2, 3, 4, 5, 6]
    };

    private static int MinDistanceFromUsed(DateOnly day, HashSet<DateOnly> usedDays)
    {
        if (usedDays.Count == 0) return 7;
        return usedDays.Min(d => Math.Abs(d.DayNumber - day.DayNumber));
    }
}
