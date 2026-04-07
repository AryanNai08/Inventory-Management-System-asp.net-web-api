using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Application.DTOs.SalesOrder
{
    public class SalesByProductReportDto
    {
        public int ProductID {  get; set; }
        public string ProductName { get; set; }
        public string Sku {  get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue {  get; set; }
    }
}
