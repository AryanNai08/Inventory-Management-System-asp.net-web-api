using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Application.DTOs.Supplier
{
    public class SupplierDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson {  get; set; }
        public string Email {  get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
