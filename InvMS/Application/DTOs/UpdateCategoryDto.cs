using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class UpdateCategoryDto
    {
        
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

