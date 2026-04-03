using Application.Common;
using Application.Interfaces;
using Application.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [Authorize(Policy = "ViewDashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<DashboardSummaryDto>>> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();
            return Ok(new APIResponse<DashboardSummaryDto>(result, "Dashboard summary fetched successfully"));
        }

        [HttpGet("low-stock")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<List<LowStockDto>>>> GetLowStockReport()
        {
            var result = await _dashboardService.GetLowStockReportAsync();
            return Ok(new APIResponse<List<LowStockDto>>(result, "Low stock report fetched successfully"));
        }

        [HttpGet("top-selling")]
        [Authorize(Policy = "ViewReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<List<TopProductDto>>>> GetTopSellingProducts([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetTopSellingProductsAsync(count);
            return Ok(new APIResponse<List<TopProductDto>>(result, "Top selling products fetched successfully"));
        }
    }
}
