using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName {  get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]

        public int RoleId { get; set; }

    }
}
