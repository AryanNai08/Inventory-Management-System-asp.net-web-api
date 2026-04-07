using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Warehouse
{
    public class WarehouseDto
    {
        //Id, Name, Location, Description
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
