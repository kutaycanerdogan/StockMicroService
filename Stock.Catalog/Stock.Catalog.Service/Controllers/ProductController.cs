using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Stock.Catalog.Contract;
using Stock.Catalog.Service.Entities;
using Stock.Common;

namespace Stock.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> productRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ProductController(IRepository<Product> productRepository, IPublishEndpoint publishEndpoint)
        {
            this.productRepository = productRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<Product[]>> GetAllAsync()
        {
            var stocks = await productRepository.GetAllAsync();
            var stocksArr = stocks.ToArray();

            return stocksArr;
        }

        [HttpGet("{productCode}")]
        public async Task<ActionResult<Product>> GetByProductCodeAsync(string productCode)
        {
            var all = await productRepository.GetAllAsync();
            var product = await productRepository.GetAsync(x => x.ProductCode == productCode);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostAsync(Product model)
        {
            var product = new Product
            {
                Id = model.Id.ToString() != "00000000-0000-0000-0000-000000000000" ? model.Id : Guid.NewGuid(),
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                CreatedDate = model.CreatedDate,
            };

            await productRepository.CreateAsync(product);
            return CreatedAtAction(nameof(GetByProductCodeAsync), new { productCode = product.ProductCode }, product);
            // return await GetByProductCodeAsync(product.ProductCode);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, Product updateProduct)
        {
            var existingProduct = await productRepository.GetAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductCode = updateProduct.ProductCode;
            existingProduct.ProductName = updateProduct.ProductName;

            await productRepository.UpdateAsync(existingProduct);

            await publishEndpoint.Publish(new ProductUpdated { Id = existingProduct.Id, ProductName = existingProduct.ProductName, ProductCode = existingProduct.ProductCode });

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await productRepository.RemoveAsync(id);
            await publishEndpoint.Publish(new ProductDeleted { ProductId = id });

            return NoContent();
        }
    }
}