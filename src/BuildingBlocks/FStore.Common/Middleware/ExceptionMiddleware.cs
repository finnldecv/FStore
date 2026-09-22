using System.Net;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using FStore.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace FStore.Common.Middleware;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  public ExceptionMiddleware(RequestDelegate next) => _next = next;
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (NotFoundException ex)
    {
      await WriteError(context, HttpStatusCode.NotFound, ex.Message);
    }
    catch (AppException ex)
    {
      await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
    }
    catch (Exception ex)
    {
      await WriteError(context, HttpStatusCode.InternalServerError, ex.Message);
    }
  }

  private static async Task WriteError(HttpContext context, HttpStatusCode code, string message)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)code;
    var payload = JsonSerializer.Serialize(new { error = message });
    await context.Response.WriteAsync(payload);
  }
}