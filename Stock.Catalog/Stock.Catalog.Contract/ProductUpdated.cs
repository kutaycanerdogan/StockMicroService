using System;

namespace Stock.Catalog.Contract
{
    public class ProductUpdated
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}