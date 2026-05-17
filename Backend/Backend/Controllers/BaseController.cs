using System.Security.Claims;
using Backend.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid GetUserIdFromClaims()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.Sid);
        return Guid.TryParse(claimValue, out var id) ? id : Guid.Empty;
    }

    protected IActionResult HandleError<T>(BaseResponse<T> result)
    {
        return result.Message switch
        {
            nameof(ErrorEnums.UnknownError) => StatusCode(500, result),
            nameof(ErrorEnums.UserNotFound) => NotFound(result),
            nameof(ErrorEnums.NotFound) => NotFound(result),
            nameof(ErrorEnums.SessionNotFound) => NotFound(result),
            nameof(ErrorEnums.ValidationError) => BadRequest(result),
            nameof(ErrorEnums.Forbidden) => StatusCode(403, result),
            _ => StatusCode(500, result)
        };
    }
}
