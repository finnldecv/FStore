using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FStore.Identity.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName,
  string Role="Customer") : IRequest<string>;