using Application.Common;
using Application.DTOs.SalesOrder;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Common;
using Application.Interfaces.SalesOrder;

namespace InvMS.Controller
{
    [Route("api/sales-orders")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrdersController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        [HttpGet]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<PaginatedResult<SalesOrderDto>>>> GetAll([FromQuery] PaginationParams @params)
        {
            var result = await _salesOrderService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<SalesOrderDto>>(result, "Sales orders fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetSalesOrderById")]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<SalesOrderDto>>> GetById(int id)
        {
            var result = await _salesOrderService.GetByIdAsync(id);
            return Ok(new APIResponse<SalesOrderDto>(result, "Sales order fetched successfully"));
        }

        [HttpPost]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<SalesOrderDto>>> Create([FromBody] CreateSalesOrderDto dto)
        {
            var result = await _salesOrderService.CreateAsync(dto);
            return Ok(new APIResponse<SalesOrderDto>(result, "Sales order created successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<SalesOrderDto>>> Update(int id, [FromBody] UpdateSalesOrderDto dto)
        {
            var result = await _salesOrderService.UpdateAsync(id, dto);
            return Ok(new APIResponse<SalesOrderDto>(result, "Sales order updated successfully"));
        }

        [HttpPatch]
        [Route("{id:int}/confirm", Name = "ConfirmSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<bool>>> Confirm(int id)
        {
            var result = await _salesOrderService.ConfirmAsync(id);
            return Ok(new APIResponse<bool>(result, "Sales order confirmed successfully"));
        }

        [HttpPatch]
        [Route("{id:int}/ship", Name = "ShipSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<bool>>> Ship(int id)
        {
            var result = await _salesOrderService.ShipAsync(id);
            return Ok(new APIResponse<bool>(result, "Sales order shipped successfully"));
        }

        [HttpPatch]
        [Route("{id:int}/deliver", Name = "DeliverSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<bool>>> Deliver(int id)
        {
            var result = await _salesOrderService.DeliverAsync(id);
            return Ok(new APIResponse<bool>(result, "Sales order delivered successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "CancelSalesOrder")]
        [Authorize(Policy = "DeleteSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<bool>>> Cancel(int id)
        {
            var result = await _salesOrderService.CancelAsync(id);
            return Ok(new APIResponse<bool>(result, "Sales order canceled successfully"));
        }

        [HttpGet]
        [Route("search")]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<List<SalesOrderDto>>>> Search(
            [FromQuery] int? status, 
            [FromQuery] int? customerId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            var result = await _salesOrderService.SearchAsync(status, customerId, startDate, endDate);
            return Ok(new APIResponse<List<SalesOrderDto>>(result, "Sales orders fetched successfully"));
        }
    }
}
