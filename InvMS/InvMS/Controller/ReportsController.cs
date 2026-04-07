using Application.Common;
using Domain.Interfaces;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Application.Interfaces.Dashboard;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IReportPDFService _pdfService;

        public ReportsController(IDashboardService dashboardService, IReportPDFService pdfService)
        {
            _dashboardService = dashboardService;
            _pdfService = pdfService;
        }

        [HttpGet("sales-by-product")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetSalesByProduct(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
        {
            var data = await _dashboardService.GetSalesByProductReportAsync(startDate, endDate);

            var pdf = _pdfService.GeneratePdf("SalesByProduct", "dsSalesByProduct", data);

            return File(pdf, "application/pdf", $"Sales_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("purchases-by-supplier")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetPurchasesBySupplier(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var data = await _dashboardService.GetPurchasesBySupplierReportAsync(startDate, endDate);
            var pdf = _pdfService.GeneratePdf("PurchasesBySupplier", "dsPurchasesBySupplier", data);
            return File(pdf, "application/pdf", $"Purchases_Supplier_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("stock-movement")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetStockMovement([FromQuery] int? year)
        {
            int reportYear = year ?? DateTime.UtcNow.Year;
            var data = await _dashboardService.GetStockMovementReportAsync(reportYear);
            var pdf = _pdfService.GeneratePdf("StockMovement", "dsStockMovement", data);
            return File(pdf, "application/pdf", $"Stock_Movement_{reportYear}.pdf");
        }

        [HttpGet("revenue")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetRevenue(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var data = await _dashboardService.GetRevenueReportAsync(startDate, endDate);
            // Wrap single object in a list for RDLC data source
            var pdf = _pdfService.GeneratePdf("Revenue", "dsRevenue", new[] { data });
            return File(pdf, "application/pdf", $"Revenue_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("order-status-summary")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetOrderStatusSummary()
        {
            var data = await _dashboardService.GetOrderStatusSummaryAsync();
            var pdf = _pdfService.GeneratePdf("OrderStatusSummary", "dsOrderStatusSummary", data);
            return File(pdf, "application/pdf", $"Order_Status_Summary_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
