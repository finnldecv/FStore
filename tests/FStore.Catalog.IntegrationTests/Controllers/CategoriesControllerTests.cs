using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.Application.Features.Categories.Commands.DeleteCategory;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.IntegrationTests.Fixtures;

namespace FStore.Catalog.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<CatalogApiFactory>
{
  private readonly HttpClient _client;
  public CategoriesControllerTests(CatalogApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task CreateThenGetAllContainsCategory()
  {
    var response = await _client.PostAsJsonAsync("/api/Categories",
      new CreateCategoryCommand("Electronics")
    );
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var categoryId = await response.Content.ReadFromJsonAsync<Guid>();
    var getResponse = await _client.GetAsync($"/api/categories/{categoryId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
    category!.Name.Should().NotBeEmpty();
  }

  [Fact]
  public async Task DeleteCategoryWithProductsReturnsBadRequest()
  {
    var categoryResponse = await _client.PostAsJsonAsync("/api/categories",
      new CreateCategoryCommand("Electronics")
    );
    var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();
    
    await _client.PostAsJsonAsync("/api/products",
      new CreateProductCommand("Laptop", "Gaming laptop", 1499.99m, 5, categoryId)
    );

    var deleteResponse = await _client.DeleteAsync($"/api/categories/{categoryId}");
    deleteResponse.StatusCode.Should().BeOneOf(
      HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableContent
    );
  }
}