using Application.Common;
using Application.DTOs.Warehouse;
using Application.Interfaces.Warehouse;
using Domain.Interfaces;
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
        public WarehousesController(IWarehouseService warehouseService)
        {
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
        public async Task<ActionResult<APIResponse<IEnumerable<WarehouseDto>>>> GetAllWarehouses()
        {
            var result = await _warehouseService.GetAllAsync();
            return Ok(new APIResponse<IEnumerable<WarehouseDto>>(result, "Warehouses fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetWarehouseById")]
        [Authorize(Policy = "ViewWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<WarehouseDto>>> GetWarehouseById(int id)
        {
            var result = await _warehouseService.GetByIdAsync(id);
            return Ok(new APIResponse<WarehouseDto>(result, "Warehouse fetched successfully"));
        }

        [HttpPost]
        [Route("CreateWarehouse")]
        [Authorize(Policy = "ManageWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<WarehouseDto>>> CreateWarehouse([FromBody] CreateWarehouseDto dto)
        {
            var result = await _warehouseService.CreateAsync(dto);
            return CreatedAtRoute("GetWarehouseById", new { id = result.Id }, new APIResponse<WarehouseDto>(result, "Warehouse created successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateWarehouse")]
        [Authorize(Policy = "ManageWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto dto)
        {
            var result = await _warehouseService.UpdateAsync(id, dto);
            return Ok(new APIResponse<bool>(result, "Warehouse updated successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteWarehouse")]
        [Authorize(Policy = "DeleteWarehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "Warehouse deleted successfully"));
        }
    }
}
