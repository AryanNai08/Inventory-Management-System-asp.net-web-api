using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Enter username")]
        public string Username { get; set; }
        [Required(ErrorMessage ="Enter password")]
        public string Password { get; set; }
    }
}
