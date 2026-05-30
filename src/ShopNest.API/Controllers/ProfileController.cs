using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Notifications.Queries.GetAddresses;
using ShopNest.Application.Features.Notifications.Queries.GetProfile;
using ShopNest.Application.Features.Users.Commands.AddAddress;
using ShopNest.Application.Features.Users.Commands.DeleteAddress;
using ShopNest.Application.Features.Users.Commands.SetDefaultAddress;
using ShopNest.Application.Features.Users.Commands.UpdateAddress;
using ShopNest.Application.Features.Users.Commands.UpdateProfile;
using ShopNest.Application.Features.Users.Commands.UploadAvatar;

namespace ShopNest.API.Controllers;

[Authorize]
[Route("api/v1/profile")]
public sealed class ProfileController : BaseApiController
{
    /// GET /api/v1/profile
    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetProfileQuery(), ct));

    /// PUT /api/v1/profile
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileCommand cmd,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(cmd, ct));

    /// POST /api/v1/profile/avatar
    [HttpPost("avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadAvatar(
        [FromForm] IFormFile file,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new UploadAvatarCommand(file), ct));

    // ── Addresses ──────────────────────────────────────────────────

    /// GET /api/v1/profile/addresses
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetAddressesQuery(), ct));

    /// POST /api/v1/profile/addresses
    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress(
        [FromBody] AddAddressCommand cmd,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(cmd, ct));

    /// PUT /api/v1/profile/addresses/{id}
    [HttpPut("addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateAddressRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new UpdateAddressCommand(
            id, req.FullName, req.Line1, req.Line2, req.City,
            req.State, req.PostalCode, req.Country, req.Phone), ct));

    /// DELETE /api/v1/profile/addresses/{id}
    [HttpDelete("addresses/{id:guid}")]
    public async Task<IActionResult> DeleteAddress(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new DeleteAddressCommand(id), ct));

    /// PATCH /api/v1/profile/addresses/{id}/default
    [HttpPatch("addresses/{id:guid}/default")]
    public async Task<IActionResult> SetDefault(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new SetDefaultAddressCommand(id), ct));
}

public sealed record UpdateAddressRequest(
    string  FullName, string Line1, string? Line2,
    string  City,     string State,  string PostalCode,
    string  Country,  string? Phone
);
