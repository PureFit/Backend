using Backend.Application.DTOs.TrainingSet;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/sets")]
    public class TrainingSetController : BaseController
    {
        private readonly ITrainingSetService _trSetService;

        public TrainingSetController(ITrainingSetService trSetService)
        {
            _trSetService = trSetService;
        }

        // ── TrainingSet ───────────────────────────────────────────────────────

        [HttpGet("{setId}")]
        public async Task<IActionResult> GetSetById(Guid setId)
        {
            var result = await _trSetService.GetTrainingSetByIdAsync(setId, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSets([FromQuery] TrainingSetFilter filter)
        {
            var result = await _trSetService.GetTrainingSetsByFilterAsync(filter, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSet([FromBody] CreateSetRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            var result = await _trSetService.CreateTrainingSet(request);
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpPatch("{setId}")]
        public async Task<IActionResult> UpdateSet(Guid setId, [FromBody] UpdateSetRequest request)
        {
            request.SetId = setId;
            var result = await _trSetService.UpdateTrainingSet(request, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpDelete("{setId}")]
        public async Task<IActionResult> DeleteSet(Guid setId)
        {
            var result = await _trSetService.DeleteTrainingSet(setId, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        // ── SetBlock ──────────────────────────────────────────────────────────

        [HttpPost("{setId}/blocks")]
        public async Task<IActionResult> AddBlock(Guid setId, [FromBody] AddSetBlockRequest request)
        {
            request.SetId = setId;
            request.UserId = GetUserIdFromClaims();
            var result = await _trSetService.AddSetBlock(request);
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpPatch("{setId}/blocks/{blockId}")]
        public async Task<IActionResult> UpdateBlock(Guid setId, Guid blockId, [FromBody] UpdateSetBlockRequest request)
        {
            request.SetId = setId;
            request.BlockId = blockId;
            var result = await _trSetService.UpdateSetBlock(request, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpDelete("{setId}/blocks/{blockId}")]
        public async Task<IActionResult> DeleteBlock(Guid setId, Guid blockId)
        {
            var result = await _trSetService.DeleteSetBlock(blockId, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        // ── ExerciseEntry ─────────────────────────────────────────────────────

        [HttpPost("{setId}/blocks/{blockId}/entries")]
        public async Task<IActionResult> AddEntry(Guid setId, Guid blockId, [FromBody] AddExerciseEntryToSetBlockRequest request)
        {
            request.SetId = setId;
            request.BlockId = blockId;
            request.UserId = GetUserIdFromClaims();
            var result = await _trSetService.AddExerciseEntryToSetBlock(request);
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpPatch("{setId}/blocks/{blockId}/entries/{entryId}")]
        public async Task<IActionResult> UpdateEntry(Guid setId, Guid blockId, Guid entryId, [FromBody] UpdateSetBlockExerciseEntryRequest request)
        {
            request.SetId = setId;
            request.BlockId = blockId;
            request.EntryId = entryId;
            var result = await _trSetService.UpdateSetBlockExerciseEntry(request, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }

        [HttpDelete("{setId}/blocks/{blockId}/entries/{entryId}")]
        public async Task<IActionResult> DeleteEntry(Guid setId, Guid blockId, Guid entryId)
        {
            var result = await _trSetService.DeleteExerciseEntry(entryId, GetUserIdFromClaims());
            return result.Success ? Ok(result) : HandleError(result);
        }
    }
}
