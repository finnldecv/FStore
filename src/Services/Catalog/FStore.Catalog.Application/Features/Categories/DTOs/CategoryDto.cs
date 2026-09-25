namespace FStore.Catalog.Application.Features.Categories.DTOs;

public record CategoryDto(
  Guid Id,
  string Name,
  int ProductCount
);