using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Customer
{
    public class CreateCustomerDto
    {
        //Name(required), Email, Phone, Address, City
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
