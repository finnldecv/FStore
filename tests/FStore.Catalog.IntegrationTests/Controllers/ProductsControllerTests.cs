using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;
using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Catalog.IntegrationTests.Fixtures;

namespace FStore.Catalog.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CatalogApiFactory>, IAsyncLifetime
{
  private readonly CatalogApiFactory _factory;
  private readonly HttpClient _client;

  public ProductsControllerTests(CatalogApiFactory factory)
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
        "/api/categories", new CreateCategoryCommand("Electronics"));
    var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
    var categoryId = category!.Id;

    var createCommand = new CreateProductCommand(
        "Laptop", "Gaming laptop", 1499.99m, 5, categoryId);

    var createResponse = await _client.PostAsJsonAsync("/api/products", createCommand);
    createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var product = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
    var productId = product!.Id;

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var productGet = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
    productGet.Should().NotBeNull();
    productGet!.Name.Should().Be("Laptop");
    productGet.Price.Should().Be(1499.99m);
    productGet.CategoryId.Should().Be(categoryId);
    productGet.StockQuantity.Should().Be(5);
  }

  [Fact]
  public async Task UpdateWithExistingProductReturnsNoContent()
  {
    var categoryResponse = await _client.PostAsJsonAsync(
        "/api/categories", new CreateCategoryCommand("Electronics"));
    var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
    var categoryId = category!.Id;

    var createResponse = await _client.PostAsJsonAsync("/api/products",
        new CreateProductCommand("Laptop", "X", 100m, 1, categoryId));
    var product = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
    var productId = product!.Id;

    var updateCommand = new UpdateProductCommand(
        productId, "Laptop pro", "Updated", 150m, 3, categoryId);

    var updateResponse = await _client.PutAsJsonAsync(
        $"/api/products/{productId}", updateCommand);

    updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    var productGet = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
    productGet!.Name.Should().Be("Laptop pro");
    productGet.Price.Should().Be(150m);
    productGet.StockQuantity.Should().Be(3);
  }

  [Fact]
  public async Task DeleteWithExistingProductReturnsNoContent()
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

    var deleteResponse = await _client.DeleteAsync($"/api/products/{productId}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/products/{productId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetByIdWithUnknownIdReturnsNotFound()
  {
    var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task CreateWithInvalidDataReturnsBadRequest()
  {
    var invalid = new CreateProductCommand("", "X", -1m, -1, Guid.NewGuid());

    var response = await _client.PostAsJsonAsync("/api/products", invalid);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}