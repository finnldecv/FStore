using MediatR;

namespace FStore.Identity.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<string>;