using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Warehouse
{
    public class CreateWarehouseDto
    {
        //Name(required), Location, Description
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
