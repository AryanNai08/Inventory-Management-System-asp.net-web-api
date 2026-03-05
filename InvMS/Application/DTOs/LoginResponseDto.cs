using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public List<string>? Roles { get; set; }
    }
}
