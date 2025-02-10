using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using TrueCode.Services.Auth;

namespace TrueCode.API.Web.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ApiController
{
    private readonly AuthService.AuthServiceClient _grpcClient;

    public AuthController(IConfiguration config)
    {
        var url = config["SERVICE_AUTH_URL"];

        if (string.IsNullOrEmpty(url))
        {
            throw new NullReferenceException("SERVICE_AUTH_URL is empty");
        }
        
        var channel = GrpcChannel.ForAddress(url);
        _grpcClient = new AuthService.AuthServiceClient(channel);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _grpcClient.RegisterAsync(request);
            return StatusCode(StatusCodes.Status201Created, new { message = response.Message });
        }
        catch (RpcException e)
        {
            return HandleGrpcException(e);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        try
        {
            var response = await _grpcClient.AuthenticateAsync(request);
            return Ok(new
            {
                accessToken = response.AccessToken,
                refreshToken = response.RefreshToken
            });
        }
        catch (RpcException e)
        {
            return HandleGrpcException(e);
        }
    }
}