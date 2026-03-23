using Application.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly APIResponse _apiResponse;

        public ReportsController(IDashboardService dashboardService, APIResponse apiResponse)
        {
            _dashboardService = dashboardService;
            _apiResponse = apiResponse;
        }

        [HttpGet("sales-by-product")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetSalesByProduct(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            _apiResponse.Data = await _dashboardService.GetSalesByProductReportAsync(startDate, endDate);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
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
