using Application.Common;
using Application.DTOs.StockAdjustment;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockAdjustmentsController : ControllerBase
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;

        public StockAdjustmentsController(IStockAdjustmentService stockAdjustmentService)
        {
            _stockAdjustmentService = stockAdjustmentService;
        }

        [HttpGet]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<PaginatedResult<StockAdjustmentDto>>>> GetAll([FromQuery] PaginationParams @params)
        {
            var result = await _stockAdjustmentService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<StockAdjustmentDto>>(result, "Stock adjustments fetched successfully"));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<StockAdjustmentDto>>> GetById(int id)
        {
            var result = await _stockAdjustmentService.GetByIdAsync(id);
            return Ok(new APIResponse<StockAdjustmentDto>(result, "Stock adjustment fetched successfully"));
        }

        [HttpGet("product/{productId:int}")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<List<StockAdjustmentDto>>>> GetByProductId(int productId)
        {
            var result = await _stockAdjustmentService.GetByProductIdAsync(productId);
            return Ok(new APIResponse<List<StockAdjustmentDto>>(result, "Stock adjustments fetched successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "ManageStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<StockAdjustmentDto>>> Create([FromBody] CreateStockAdjustmentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new APIResponse<StockAdjustmentDto>(null, "User is not authenticated") 
                { 
                    Status = false, 
                    StatusCode = HttpStatusCode.Unauthorized, 
                });
            }
            
            int userId = int.Parse(userIdClaim.Value);
            
            var result = await _stockAdjustmentService.CreateAsync(dto, userId);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new APIResponse<StockAdjustmentDto>(result, "Stock adjustment created successfully"));
        }

        [HttpGet("types")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<List<AdjustmentTypeDto>>>> GetAdjustmentTypes()
        {
            var result = await _stockAdjustmentService.GetAdjustmentTypesAsync();
            return Ok(new APIResponse<List<AdjustmentTypeDto>>(result, "Adjustment types fetched successfully"));
        }
    }
}
