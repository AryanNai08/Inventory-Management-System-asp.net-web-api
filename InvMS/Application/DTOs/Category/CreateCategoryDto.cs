using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
