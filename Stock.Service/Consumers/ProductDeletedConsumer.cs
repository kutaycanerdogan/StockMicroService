using System.Threading.Tasks;
using MassTransit;
using Stock.Catalog.Contract;
using Stock.Common;
using Stock.Service.Entities;

namespace Stock.Service.Consumers
{
    public class ProductDeletedConsumer : IConsumer<ProductDeleted>
    {
        private readonly IRepository<Inventory> repository;

        public ProductDeletedConsumer(IRepository<Inventory> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            var message = context.Message;

            var items = await repository.GetAllAsync(x => x.ProductId == message.ProductId);
            foreach (var item in items)
            {
                await repository.RemoveAsync(item.Id);
            }
        }
    }
}