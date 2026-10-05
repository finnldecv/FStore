using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;
using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.IntegrationTests.Fixtures;

namespace FStore.Catalog.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CatalogApiFactory>
{
  private readonly HttpClient _client;

  public ProductsControllerTests(CatalogApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task GetAllReturnsEmptyListWhenNoProducts()
  {
    var response = await _client.GetAsync("/api/products");
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
    products.Should().NotBeNull();
    products!.Should().BeEmpty();
  }

  [Fact]
  public async Task CreateThenGetByIdReturnsProduct()
  {
    var categoryResponse = await _client.PostAsJsonAsync(
      "/api/categories", new CreateCategoryCommand("Electronics")
    );
    var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

    var createCommand = new CreateProductCommand(
      "Laptop", "Gaming laptop", 1499.99m, 5, categoryId
    );

    var createResponse = await _client.PostAsJsonAsync("/api/products", createCommand);
    createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
    product.Should().NotBeNull();
    product!.Name.Should().Be("Laptop");
    product.Price.Should().Be(1499.99m);
    product.CategoryId.Should().Be(categoryId);

  }

  [Fact]
  public async Task UpdateWithExistingProductReturnsNoContent()
  {
    var categoryResponse = await _client.PostAsJsonAsync(
      "/api/categories", new CreateCategoryCommand("Electronics")
    );
    var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

    var createResponse = await _client.PostAsJsonAsync("/api/products",
      new CreateProductCommand("Laptop", "X", 100m, 1, categoryId)
    );
    var productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

    var updateCommand = new UpdateProductCommand(
      productId, "Laptop pro", "Updated", 150m, 3, categoryId
    );

    var updateResponse = await _client.PutAsJsonAsync(
      $"/api/products/{productId}", updateCommand
    );

    updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
    product!.Name.Should().Be("Laptop pro");
    product.Price.Should().Be(150m);
  }

  [Fact]
  public async Task DeleteWithExistingProductReturnsNoContent()
  {
    var categoryResponse = await _client.PostAsJsonAsync(
      "/api/categories", new CreateCategoryCommand("Electronics")
    );
    var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

    var createResponse = await _client.PostAsJsonAsync("/api/products",
      new CreateProductCommand("Laptop", "X", 100m, 1, categoryId)
    );
    var productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

    var deleteResponse = await _client.DeleteAsync($"/api/products/{productId}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    getResponse.Content.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetByIdWithUnknownIdReturnsNotFound()
  {
    var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetByIdWithInvalidIdReturnsBadRequest()
  {
    var invalid = new CreateProductCommand("", "X", -1m, -1, Guid.NewGuid());

    var response = await _client.PostAsJsonAsync("/api/products", invalid);

    response.StatusCode.Should().BeOneOf(
      HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableContent
    );
  }
}