using System;
using Stock.Common;

namespace Stock.Service.Entities
{
    public class Inventory : IEntity
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? VariantId { get; set; }
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public decimal Quantity { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}