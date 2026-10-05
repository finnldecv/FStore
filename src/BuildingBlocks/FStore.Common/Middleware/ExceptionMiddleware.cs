using System.Net;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using FStore.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using FluentValidation;

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
    catch (FluentValidation.ValidationException ex)
    {
      context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      context.Response.ContentType = "application/json";
      var errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage });
      await context.Response.WriteAsJsonAsync(new { errors });
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