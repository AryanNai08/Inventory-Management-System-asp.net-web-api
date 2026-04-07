using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Customer
{
    public class CustomerDto
    {
        //Id, Name, Email, Phone, Address, City
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
