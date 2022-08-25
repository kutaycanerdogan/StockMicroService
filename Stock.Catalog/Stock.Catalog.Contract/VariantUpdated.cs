using System;

namespace Stock.Catalog.Contract
{
    public class VariantUpdated
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}