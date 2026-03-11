using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Warehouse
{
    public class UpdateWarehouseDto
    {
        //Name(required), Location, Description
        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
