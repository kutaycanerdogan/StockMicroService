using System.Threading.Tasks;
using MassTransit;
using Stock.Catalog.Contract;
using Stock.Common;
using Stock.Service.Entities;

namespace Stock.Service.Consumers
{
    public class VariantDeletedConsumer : IConsumer<VariantDeleted>
    {
        private readonly IRepository<Inventory> repository;

        public VariantDeletedConsumer(IRepository<Inventory> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<VariantDeleted> context)
        {
            var message = context.Message;

            var items = await repository.GetAllAsync(x => x.VariantId == message.VariantId);
            foreach (var item in items)
            {
                await repository.RemoveAsync(item.Id);
            }
        }
    }
}