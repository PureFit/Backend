using Backend.Application.DTOs.Excercises;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/exercises")]
public class ExercisesController : BaseController
{
    private readonly ITrainingSetService _exerciseService;

    public ExercisesController(ITrainingSetService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetExercises([FromQuery] ExerciseFilter filter)
    {
        var result = await _exerciseService.GetExercisesAsync(filter);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExerciseById(string id)
    {
        var result = await _exerciseService.GetExerciseByIdAsync(id);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("muscles")]
    public async Task<IActionResult> GetMuscles()
    {
        var result = await _exerciseService.GetMusclesAsync();
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("body-parts")]
    public async Task<IActionResult> GetBodyParts()
    {
        var result = await _exerciseService.GetBodyPartsAsync();
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("equipments")]
    public async Task<IActionResult> GetEquipments()
    {
        var result = await _exerciseService.GetEquipmentsAsync();
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var result = await _exerciseService.GetExerciseTypesAsync();
        return result.Success ? Ok(result) : HandleError(result);
    }
}
