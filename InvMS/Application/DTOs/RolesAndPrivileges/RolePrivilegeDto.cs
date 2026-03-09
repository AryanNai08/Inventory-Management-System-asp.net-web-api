using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RolesAndPrivileges
{
    public class RolePrivilegeDto
    {
            
        [Required(ErrorMessage = "please enter a role id!")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "please enter a privilege id!")]
        public int PrivilegeId { get; set; }
    
}
}
