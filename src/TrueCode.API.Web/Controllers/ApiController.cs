using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace TrueCode.API.Web.Controllers;

public abstract class ApiController : ControllerBase
{
    protected CallOptions AuthMetadata
    {
        get
        {
            var metadata = new Metadata();
            
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var header))
            {
                metadata.Add("Authorization", header.ToString());
            }
            
            return new CallOptions(metadata);
        }
    }

    protected IActionResult HandleGrpcException(RpcException e)
    {
        return e.StatusCode switch
        {
            Grpc.Core.StatusCode.Unauthenticated => Unauthorized(new { error = e.Status.Detail }),
            Grpc.Core.StatusCode.InvalidArgument => BadRequest(new { error = e.Status.Detail }),
            Grpc.Core.StatusCode.NotFound => NotFound(new { error = e.Status.Detail }),
            Grpc.Core.StatusCode.AlreadyExists => Conflict(new { error = e.Status.Detail }),
            _ => StatusCode(500, new { error = e.Status.Detail })
        };
    }
}