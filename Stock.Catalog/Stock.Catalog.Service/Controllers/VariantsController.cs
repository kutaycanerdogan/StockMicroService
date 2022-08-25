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
    [Route("api/variant")]
    public class VariantController : ControllerBase
    {
        private readonly IRepository<Variant> variantRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public VariantController(IRepository<Variant> variantRepository, IPublishEndpoint publishEndpoint)
        {
            this.variantRepository = variantRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Variant>>> GetAsync()
        {
            var stocks = await variantRepository.GetAllAsync();

            return Ok(stocks);
        }

        [HttpGet("{variantCode}")]
        public async Task<ActionResult<Variant>> GetByIdAsync(string variantCode)
        {
            var variant = await variantRepository.GetAsync(x => x.VariantCode == variantCode);

            if (variant == null)
            {
                return NotFound();
            }

            return variant;
        }

        [HttpPost]
        public async Task<ActionResult<Variant>> PostAsync(Variant model)
        {
            var variant = new Variant
            {
                ProductId = model.ProductId,
                ProductCode = model.ProductCode,
                VariantCode = model.VariantCode,
                VariantName = model.VariantName,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await variantRepository.CreateAsync(variant);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = variant.Id }, variant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, Variant updateVariant)
        {
            var existingVariant = await variantRepository.GetAsync(id);

            if (existingVariant == null)
            {
                return NotFound();
            }

            existingVariant.ProductCode = updateVariant.ProductCode;
            existingVariant.VariantCode = updateVariant.VariantCode;
            existingVariant.VariantName = updateVariant.VariantName;

            await variantRepository.UpdateAsync(existingVariant);

            await publishEndpoint.Publish(new VariantUpdated { Id = existingVariant.Id, ProductCode = existingVariant.ProductCode, VariantCode = existingVariant.VariantCode });

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await variantRepository.RemoveAsync(id);
            await publishEndpoint.Publish(new VariantDeleted { VariantId = id });

            return NoContent();
        }
    }
}