using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
}
