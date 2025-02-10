using Grpc.Core;
using Grpc.Core.Interceptors;

namespace TrueCode.Services.Currencies.Middlewares;

public class AuthorizationInterceptor(ILogger<AuthorizationInterceptor> _logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        Console.WriteLine("AuthorizationInterceptor");
        var user = context.GetHttpContext().User;

        if (user is null || !user.Identity!.IsAuthenticated)
        {
            _logger.LogWarning("Unauthorized access attempt.");
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthorized"));
        }

        return await continuation(request, context);
    }
}