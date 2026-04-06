using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Warehouse
{
    public virtual ICollection<ProductWarehouseStock> ProductWarehouseStocks { get; set; } = new List<ProductWarehouseStock>();
}
