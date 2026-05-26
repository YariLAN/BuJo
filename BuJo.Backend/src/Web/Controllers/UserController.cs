using BuJo.Contracts.V1;
using Microsoft.AspNetCore.Mvc;

namespace BuJo.Web.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost(ApiRoutesV1.UserRegister)]
    public async Task<ActionResult<Guid>> RegisterAsync(
        UserRegisterRequest request,
        CancellationToken ct)
    {
        return Ok();
    }
}

public sealed record UserRegisterRequest(
    string TelegramId,
    string Username);