using Backend.Application.DTOs.Excercises;
using Backend.Application.DTOs.Plan;
using Backend.Application.Repositories;
using Backend.Core.Entities.ExerciseRelated;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly AppDbContext _db;

    public ExerciseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ExercisePagedResult> GetExercisesAsync(ExerciseFilter filter)
    {
        var query = _db.Exercises
            .Include(e => e.ExerciseTypes)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseBodyParts).ThenInclude(eb => eb.BodyPart)
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Keywords))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{filter.Keywords}%"));

        if (!string.IsNullOrEmpty(filter.BodyPart))
            query = query.Where(e => e.ExerciseBodyParts.Any(eb => EF.Functions.ILike(eb.BodyPart.Name, filter.BodyPart)));

        if (!string.IsNullOrEmpty(filter.Muscle))
            query = query.Where(e => e.ExerciseMuscles.Any(em => em.Muscle.Name == filter.Muscle));

        if (!string.IsNullOrEmpty(filter.Equipment))
            query = query.Where(e => e.ExerciseEquipments.Any(ee => ee.Equipment.Name == filter.Equipment));

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(e => EF.Functions.ILike(e.Category, filter.Category));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new ExercisePagedResult
        {
            Items = items.Select(e => new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                ImageUrl = e.ImageUrl,
                GifUrl = e.GifUrl,
                Category = e.Category.ToString(),
                AllowsWeight = e.AllowsWeight,
                BodyParts = e.ExerciseBodyParts.Select(eb => eb.BodyPart.Name).ToList(),
                PrimaryMuscles = e.ExerciseMuscles.Where(em => em.Role == Core.Enums.MuscleRole.Primary).Select(em => em.Muscle.Name).ToList(),
                Equipments = e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
                ExerciseTypes = e.ExerciseTypes.Select(t => new ExerciseTypeDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    MeasureCategory = t.MeasureCategory.ToString(),
                    Coefficient = t.Coefficient,
                    ReferenceSpeedKmh = t.ReferenceSpeedKmh,
                    AverageRepsPerMinute = t.AverageRepsPerMinute
                }).ToList()
            }).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ExerciseDetailsDto?> GetExerciseByIdAsync(Guid id)
    {
        var e = await _db.Exercises
            .Include(e => e.ExerciseTypes)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseBodyParts).ThenInclude(eb => eb.BodyPart)
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e is null) return null;

        var relatedIds = (e.RelatedExerciseIds ?? [])
            .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
            .Where(g => g.HasValue).Select(g => g!.Value).ToList();
        var related = await _db.Exercises
            .Include(r => r.ExerciseTypes)
            .Include(r => r.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(r => r.ExerciseBodyParts).ThenInclude(eb => eb.BodyPart)
            .Include(r => r.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .Where(r => relatedIds.Contains(r.Id))
            .ToListAsync();

        return new ExerciseDetailsDto
        {
            Id = e.Id,
            Name = e.Name,
            ImageUrl = e.ImageUrl,
            GifUrl = e.GifUrl,
            Overview = e.Overview,
            Category = e.Category.ToString(),
            AllowsWeight = e.AllowsWeight,
            BaseWeightBodyRatio = e.BaseWeightBodyRatio,
            BodyParts = e.ExerciseBodyParts.Select(eb => eb.BodyPart.Name).ToList(),
            PrimaryMuscles = e.ExerciseMuscles.Where(em => em.Role == Core.Enums.MuscleRole.Primary).Select(em => em.Muscle.Name).ToList(),
            SecondaryMuscles = e.ExerciseMuscles.Where(em => em.Role == Core.Enums.MuscleRole.Secondary).Select(em => em.Muscle.Name).ToList(),
            Equipments = e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
            ExerciseTypes = e.ExerciseTypes.Select(t => new ExerciseTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                MeasureCategory = t.MeasureCategory.ToString(),
                Coefficient = t.Coefficient,
                ReferenceSpeedKmh = t.ReferenceSpeedKmh,
                AverageRepsPerMinute = t.AverageRepsPerMinute
            }).ToList(),
            Instructions = e.Instructions,
            Tips = e.Tips,
            Variations = e.Variations,
            Keywords = e.Keywords,
            RelatedExercises = related.Select(r => new ExerciseDto
            {
                Id = r.Id,
                Name = r.Name,
                ImageUrl = r.ImageUrl,
                GifUrl = r.GifUrl,
                Category = r.Category,
                AllowsWeight = r.AllowsWeight,
                BodyParts = r.ExerciseBodyParts.Select(eb => eb.BodyPart.Name).ToList(),
                PrimaryMuscles = r.ExerciseMuscles.Where(em => em.Role == Core.Enums.MuscleRole.Primary).Select(em => em.Muscle.Name).ToList(),
                Equipments = r.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
                ExerciseTypes = r.ExerciseTypes.Select(t => new ExerciseTypeDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    MeasureCategory = t.MeasureCategory.ToString(),
                    Coefficient = t.Coefficient,
                    ReferenceSpeedKmh = t.ReferenceSpeedKmh,
                    AverageRepsPerMinute = t.AverageRepsPerMinute
                }).ToList()
            }).ToList()
        };
    }

    public async Task<List<BodyPartItemDto>> GetBodyPartsAsync()
    {
        return await _db.BodyParts
            .Include(b => b.Muscles)
            .OrderBy(b => b.Name)
            .Select(b => new BodyPartItemDto
            {
                Id = b.Id,
                Name = b.Name,
                ImageUrl = b.ImageUrl,
                Muscles = b.Muscles.Select(m => new MuscleDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    ImageUrl = m.ImageUrl
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<List<EquipmentItemDto>> GetEquipmentsAsync()
    {
        return await _db.Equipments
            .OrderBy(e => e.Name)
            .Select(e => new EquipmentItemDto
            {
                Id = e.Id,
                Name = e.Name,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<List<MuscleDto>> GetMusclesAsync()
    {
        return await _db.Muscles
            .OrderBy(m => m.Name)
            .Select(m => new MuscleDto
            {
                Id = m.Id,
                Name = m.Name,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<ExerciseType?> GetExerciseTypeAsync(Guid typeId) =>
        await _db.ExerciseTypes.FirstOrDefaultAsync(t => t.Id == typeId);

    public async Task<List<ExerciseBrief>> GetExercisesBriefAsync() =>
        await _db.Exercises
            .Include(e => e.ExerciseTypes)
            .Include(e => e.ExerciseMuscles).ThenInclude(em => em.Muscle)
            .Include(e => e.ExerciseBodyParts).ThenInclude(eb => eb.BodyPart)
            .Include(e => e.ExerciseEquipments).ThenInclude(ee => ee.Equipment)
            .Select(e => new ExerciseBrief
            {
                Id = e.Id,
                Name = e.Name,
                BodyParts = e.ExerciseBodyParts.Select(eb => eb.BodyPart.Name).ToList(),
                Muscles = e.ExerciseMuscles.Select(em => em.Muscle.Name).ToList(),
                Equipment = e.ExerciseEquipments.Select(ee => ee.Equipment.Name).ToList(),
                Measure = e.ExerciseTypes.Select(t => t.MeasureCategory.ToString()).FirstOrDefault() ?? "Reps"
            })
            .ToListAsync();
}
