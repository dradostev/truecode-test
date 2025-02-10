using CurrencyService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace TrueCode.API.Web.Controllers;

[Route("api/currency")]
[ApiController]
public class CurrencyController : ApiController
{
    private readonly CurrencyService.CurrencyService.CurrencyServiceClient _grpcClient;

    public CurrencyController()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5090");
        _grpcClient = new CurrencyService.CurrencyService.CurrencyServiceClient(channel);
    }

    [HttpGet("favorites/{userId}")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        try
        {
            var request = new UserRequest { UserId = userId };
            var response = await _grpcClient.GetFavoritesAsync(request, AuthMetadata);

            return Ok(response.Currencies);
        }
        catch (RpcException e)
        {
            return HandleGrpcException(e);
        }
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        try
        {
            var response = await _grpcClient.AddFavoriteAsync(request, AuthMetadata);
            return StatusCode(StatusCodes.Status201Created, new { message = response.Message });
        }
        catch (RpcException e)
        {
            return HandleGrpcException(e);
        }
    }

    [HttpDelete("favorites")]
    public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteRequest request)
    {
        try
        {
            var response = await _grpcClient.RemoveFavoriteAsync(request, AuthMetadata);
            return Accepted(new { message = response.Message });
        }
        catch (RpcException e)
        {
            return HandleGrpcException(e);
        }
    }
}