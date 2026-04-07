using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RolesAndPrivileges
{
    public class PrivilegeDto
    {
        [Required(ErrorMessage = "please enter a privilege name!")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
