using System;
using Stock.Common;

namespace Stock.Catalog.Service.Entities
{
    public class Product : IEntity
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}