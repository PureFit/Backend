using Backend.Application.Common;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IImageRetrieverService _imageService;

    public MediaController(IImageRetrieverService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest(BaseResponse<string>.Fail(ErrorEnums.ValidationError));

        using var stream = image.OpenReadStream();
        var url = await _imageService.UploadImageAsync(stream, Guid.NewGuid().ToString(), "set-images");
        return Ok(BaseResponse<string>.Ok(url));
    }
}
