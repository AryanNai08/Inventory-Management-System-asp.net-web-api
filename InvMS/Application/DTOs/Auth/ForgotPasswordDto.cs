using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
