using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.Application.Features.Categories.Commands.DeleteCategory;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.IntegrationTests.Fixtures;
using FStore.Catalog.Application.Features.Products.DTOs;

namespace FStore.Catalog.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<CatalogApiFactory>, IAsyncLifetime
{
  private readonly HttpClient _client;
  private readonly CatalogApiFactory _factory;
  public CategoriesControllerTests(CatalogApiFactory factory)
  {
    _factory = factory;
    _client = factory.CreateClient();
  }

  public Task InitializeAsync()
  {
    return _factory.ResetDatabaseAsync();
  }

  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CreateThenGetAllContainsCategory()
  {
    var response = await _client.PostAsJsonAsync("/api/Categories",
      new CreateCategoryCommand("Electronics")
    );

    if (response.StatusCode != HttpStatusCode.Created)
    {
      var body = await response.Content.ReadAsStringAsync();
      throw new Exception($"POST /api/categories → {(int)response.StatusCode}\n{body}");
    }
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
    created.Should().NotBeNull();
    created!.Id.Should().NotBe(Guid.Empty);

    var getResponse = await _client.GetAsync($"/api/categories/{created.Id}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var category = await getResponse.Content.ReadFromJsonAsync<CategoryDto>();
    category.Should().NotBeNull();
    category!.Name.Should().Be("Electronics");
  }

  [Fact]
  public async Task DeleteCategoryWithProductsReturnsBadRequest()
  {
    var categoryResponse = await _client.PostAsJsonAsync(
        "/api/categories", new CreateCategoryCommand("Electronics"));
    var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
    var categoryId = category!.Id;

    var createResponse = await _client.PostAsJsonAsync("/api/products",
        new CreateProductCommand("Laptop", "X", 100m, 1, categoryId));

    createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var product = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
    var productId = product!.Id;
    productId.Should().NotBe(Guid.Empty);

    var deleteResponse = await _client.DeleteAsync($"/api/categories/{categoryId}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}