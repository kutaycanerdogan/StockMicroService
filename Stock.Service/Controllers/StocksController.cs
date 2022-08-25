using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Stock.Common;
using Stock.Service.Entities;

namespace Stock.Service.Controllers
{
    [ApiController]
    [Route("api/stocks")]
    public class StocksController : ControllerBase
    {
        private readonly IRepository<Inventory> productInventoryRepository;
        private readonly IRepository<Inventory> variantInventoryRepository;

        public StocksController(IRepository<Inventory> productInventoryRepository, IRepository<Inventory> variantInventoryRepository)
        {
            this.productInventoryRepository = productInventoryRepository;
            this.variantInventoryRepository = variantInventoryRepository;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetByIdAsync(Guid id)
        {
            var inventory = await variantInventoryRepository.GetAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }
        [HttpGet("{variantCode}/variant")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllVariantsAsync(string variantCode)
        {
            var stocks = await variantInventoryRepository.GetAllAsync(x => x.VariantCode == variantCode);

            return Ok(stocks);
        }
        [HttpGet("{productCode}/product")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllProductsAsync(string productCode)
        {
            var stocks = await productInventoryRepository.GetAllAsync(x => x.ProductCode == productCode);

            return Ok(stocks);
        }
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostAsync(Inventory model)
        {
            var inventory = await variantInventoryRepository.GetAsync(item => item.ProductCode == model.ProductCode && item.VariantCode == model.VariantCode);
            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = model.ProductId,
                    VariantId = model.VariantId,
                    ProductCode = model.ProductCode,
                    VariantCode = model.VariantCode,
                    Quantity = model.Quantity,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await variantInventoryRepository.CreateAsync(inventory);
            }
            else
            {
                inventory.Quantity += model.Quantity;
                inventory.ProductId = model.ProductId;
                inventory.VariantId = model.VariantId;
                inventory.ProductCode = model.ProductCode;
                inventory.VariantCode = model.VariantCode;
                await variantInventoryRepository.UpdateAsync(inventory);
            }
            return CreatedAtAction(nameof(GetByIdAsync), new { id = inventory.Id }, inventory);
        }
    }
}