using System.Threading.Tasks;
using MassTransit;
using Stock.Catalog.Contract;
using Stock.Common;
using Stock.Service.Entities;

namespace Stock.Service.Consumers
{
    public class VariantUpdatedConsumer : IConsumer<VariantUpdated>
    {
        private readonly IRepository<Inventory> repository;


        public VariantUpdatedConsumer(IRepository<Inventory> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<VariantUpdated> context)
        {
            var message = context.Message;

            var items = await repository.GetAllAsync(x => x.VariantId == message.Id);
            foreach (var item in items)
            {
                item.VariantId = message.Id;
                item.VariantCode = message.VariantCode;
                item.ProductId = message.ProductId;
                item.ProductCode = message.ProductCode;
                await repository.UpdateAsync(item);
            }
        }
    }
}