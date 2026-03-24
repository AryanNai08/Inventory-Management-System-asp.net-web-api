using Application.Common;
using Application.DTOs.Warehouse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly APIResponse _apiResponse;
        public WarehousesController(IWarehouseService warehouseService, APIResponse apiResponse)
        {
            _apiResponse = apiResponse;
           _warehouseService = warehouseService;
        }

        [HttpGet]
        [Route("GetAllWarehouses")]
        [Authorize(Policy = "ViewWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllWarehouses()
        {
            _apiResponse.Data = await _warehouseService.GetAllAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetWarehouseById")]
        [Authorize(Policy = "ViewWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetWarehouseById(int id)
        {
            _apiResponse.Data = await _warehouseService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Route("CreateWarehouse")]
        [Authorize(Policy = "ManageWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateWarehouse([FromBody] CreateWarehouseDto dto)
        {

            _apiResponse.Data = await _warehouseService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            return CreatedAtRoute("GetWarehouseById", new { id = ((WarehouseDto)_apiResponse.Data).Id }, _apiResponse);
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateWarehouse")]
        [Authorize(Policy = "ManageWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto dto)
        {
            _apiResponse.Data = await _warehouseService.UpdateAsync(id, dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteWarehouse")]
        [Authorize(Policy = "DeleteWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteWarehouse(int id)
        {
            _apiResponse.Data = await _warehouseService.SoftDeleteAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }
    }
}
