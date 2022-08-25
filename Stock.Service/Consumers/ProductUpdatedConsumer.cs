using System.Threading.Tasks;
using MassTransit;
using Stock.Catalog.Contract;
using Stock.Common;
using Stock.Service.Entities;

namespace Stock.Service.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
    {
        private readonly IRepository<Inventory> repository;


        public ProductUpdatedConsumer(IRepository<Inventory> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ProductUpdated> context)
        {
            var message = context.Message;

            var items = await repository.GetAllAsync(x => x.ProductId == message.Id);
            foreach (var item in items)
            {
                item.ProductId = message.Id;
                item.ProductCode = message.ProductCode;
                await repository.UpdateAsync(item);
            }
        }
    }
}