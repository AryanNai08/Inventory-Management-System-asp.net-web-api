using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Otp {  get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
