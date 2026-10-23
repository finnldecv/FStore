using MediatR;

namespace FStore.Identity.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<string>;