using System;
using Stock.Common;

namespace Stock.Catalog.Service.Entities
{
    public class Variant : IEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public string VariantName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}