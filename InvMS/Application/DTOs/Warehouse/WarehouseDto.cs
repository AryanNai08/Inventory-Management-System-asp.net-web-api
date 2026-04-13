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

        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
