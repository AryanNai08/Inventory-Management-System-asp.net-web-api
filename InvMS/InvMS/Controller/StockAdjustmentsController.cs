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
        private readonly APIResponse _apiResponse;

        public StockAdjustmentsController(IStockAdjustmentService stockAdjustmentService, APIResponse apiResponse)
        {
            _stockAdjustmentService = stockAdjustmentService;
            _apiResponse = apiResponse;
        }

        [HttpGet]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAll()
        {
            _apiResponse.Data = await _stockAdjustmentService.GetAllAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetById(int id)
        {
            _apiResponse.Data = await _stockAdjustmentService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet("product/{productId:int}")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetByProductId(int productId)
        {
            _apiResponse.Data = await _stockAdjustmentService.GetByProductIdAsync(productId);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPost]
        [Authorize(Policy = "ManageStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateStockAdjustmentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new APIResponse 
                { 
                    Status = false, 
                    StatusCode = HttpStatusCode.Unauthorized, 
                    Data = "User is not authenticated" 
                });
            }
            
            int userId = int.Parse(userIdClaim.Value);
            
            var result = await _stockAdjustmentService.CreateAsync(dto, userId);
            
            _apiResponse.Data = result;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _apiResponse);
        }

        [HttpGet("types")]
        [Authorize(Policy = "ViewStockAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAdjustmentTypes()
        {
            _apiResponse.Data = await _stockAdjustmentService.GetAdjustmentTypesAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }
    }
}
