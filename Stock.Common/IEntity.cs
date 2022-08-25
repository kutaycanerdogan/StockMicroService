using System;

namespace Stock.Common
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}