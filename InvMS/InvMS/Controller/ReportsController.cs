using Application.Common;
using Application.Interfaces;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly APIResponse _apiResponse;
        private readonly IWebHostEnvironment _env;

        public ReportsController(IDashboardService dashboardService, APIResponse apiResponse, IWebHostEnvironment env)
        {
            _dashboardService = dashboardService;
            _apiResponse = apiResponse;
            _env = env;
        }

        [HttpGet("sales-by-product")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetSalesByProduct(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var data = await _dashboardService.GetSalesByProductReportAsync(startDate, endDate);
            return GeneratePdfReport("SalesByProduct", "dsSalesByProduct", data);
        }

        private IActionResult GeneratePdfReport(string reportName, string dsName, object data)
        {
            try
            {
                string reportPath = Path.Combine(_env.ContentRootPath, "Reports", $"{reportName}.rdlc");
                
                if (!System.IO.File.Exists(reportPath))
                    return NotFound($"Report template {reportName}.rdlc not found at {reportPath}");

                LocalReport localReport = new LocalReport(reportPath);
                localReport.AddDataSource(dsName, data);

                var result = localReport.Execute(RenderType.Pdf, 1);
                
                return File(result.MainStream, "application/pdf", $"{reportName}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Data = ex.Message;
                return StatusCode(500, _apiResponse);
            }
        }

        [HttpGet("purchases-by-supplier")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPurchasesBySupplier(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            _apiResponse.Data = await _dashboardService.GetPurchasesBySupplierReportAsync(startDate, endDate);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet("stock-movement")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStockMovement([FromQuery] int? year)
        {
            int reportYear = year ?? DateTime.UtcNow.Year;
            _apiResponse.Data = await _dashboardService.GetStockMovementReportAsync(reportYear);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet("revenue")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetRevenue(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            _apiResponse.Data = await _dashboardService.GetRevenueReportAsync(startDate, endDate);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet("order-status-summary")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetOrderStatusSummary()
        {
            _apiResponse.Data = await _dashboardService.GetOrderStatusSummaryAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }
    }
}
