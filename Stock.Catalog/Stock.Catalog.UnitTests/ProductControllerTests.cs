using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stock.Catalog.Service.Controllers;
using Stock.Catalog.Service.Entities;
using Stock.Common;
using Xunit;

namespace Stock.Catalog.UnitTests
{
    public class ProductControllerTests
    {
        private readonly Mock<IRepository<Product>> repositoryStub = new Mock<IRepository<Product>>();
        private readonly Mock<IPublishEndpoint> publishEndpointStub = new Mock<IPublishEndpoint>();
        private readonly Random rand = new Random();
        private readonly string[] ProductCodes = new string[1];

        [Fact]
        public async Task GetByProductCodeAsync_WithUnexistingProduct_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product)null);

            var controller = new ProductController(repositoryStub.Object, publishEndpointStub.Object);

            // Act
            var result = await controller.GetByProductCodeAsync("");

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByProductCodeAsync_WithExistingProduct_ReturnsExpectedProduct()
        {
            // Arrange
            Product expectedProduct = CreateRandomProduct();

            repositoryStub.Setup(repo => repo.GetAsync(x => x.ProductCode == "1010"))
                .ReturnsAsync(expectedProduct);

            var controller = new ProductController(repositoryStub.Object, publishEndpointStub.Object);

            // Act
            var result = await controller.GetByProductCodeAsync("1010");

            // Assert
            result.Value.Should().BeEquivalentTo(expectedProduct);
        }

        [Fact]
        public async Task GetProductsAsync_WithExistingProducts_ReturnsAllProducts()
        {
            // Arrange
            var expectedProducts = new[] { CreateRandomProduct(), CreateRandomProduct(), CreateRandomProduct() };

            repositoryStub.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedProducts);

            var controller = new ProductController(repositoryStub.Object, publishEndpointStub.Object);

            // Act
            var actualProducts = await controller.GetAllAsync();

            // Assert
            actualProducts.Value.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async Task CreateProductAsync_WithProductToCreate_ReturnsCreatedProduct()
        {
            // Arrange
            var ProductCode = CreateRandomProductCode(true);

            var ProductToCreate = new Product
            {
                ProductCode = ProductCode,
                ProductName = $"Product {ProductCode}",
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow
            };

            var controller = new ProductController(repositoryStub.Object, publishEndpointStub.Object);

            // Act
            var result = await controller.PostAsync(ProductToCreate);

            // Assert
            var createdProduct = (result.Result as CreatedAtActionResult).Value as Product;
            ProductToCreate.Should().BeEquivalentTo(
                createdProduct,
                options => options.ComparingByMembers<Product>().ExcludingMissingMembers()
            );
            createdProduct.Id.Should().NotBeEmpty();
            createdProduct.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task PutAsync_WithExistingProduct_ReturnsNoContent()
        {
            // Arrange
            Product existingProduct = CreateRandomProduct();
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingProduct);

            var ProductId = existingProduct.Id;
            var ProductCode = CreateRandomProductCode(true);
            var ProductToUpdate = new Product
            {
                ProductCode = ProductCode,
                ProductName = $"Old ProductCode: {existingProduct.ProductCode}, New ProductCode: {ProductCode}",
                Id = ProductId,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var controller = new ProductController(repositoryStub.Object, publishEndpointStub.Object);

            // Act
            var result = await controller.PutAsync(ProductId, ProductToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Product CreateRandomProduct()
        {
            var ProductCode = CreateRandomProductCode(true);
            return new Product
            {
                ProductCode = ProductCode,
                ProductName = $"Product {ProductCode}",
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
        private string CreateRandomProductCode(bool isUniqe = false)
        {
            bool isExist = true;
            string ProductCode = "";

            while (isExist)
            {
                ProductCode = rand.Next(1000, 1050).ToString();
                isExist = isUniqe ? false : ProductCodes.Any(x => x == ProductCode);
            }
            return ProductCode;
        }
    }
}
