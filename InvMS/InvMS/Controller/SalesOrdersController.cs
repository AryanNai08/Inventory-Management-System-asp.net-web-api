using Application.Common;
using Application.DTOs.SalesOrder;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InvMS.Controller
{
    [Route("api/sales-orders")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly APIResponse _apiResponse;

        public SalesOrdersController(ISalesOrderService salesOrderService, APIResponse apiResponse)
        {
            _salesOrderService = salesOrderService;
            _apiResponse = apiResponse;
        }

        [HttpGet]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAll([FromQuery] PaginationParams @params)
        {
            var result = await _salesOrderService.GetAllAsync(@params);
            _apiResponse.Data = result;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetSalesOrderById")]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetById(int id)
        {
            _apiResponse.Data = await _salesOrderService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPost]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateSalesOrderDto dto)
        {
            _apiResponse.Data = await _salesOrderService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] UpdateSalesOrderDto dto)
        {
            _apiResponse.Data = await _salesOrderService.UpdateAsync(id, dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPatch]
        [Route("{id:int}/confirm", Name = "ConfirmSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Confirm(int id)
        {
            _apiResponse.Data = await _salesOrderService.ConfirmAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPatch]
        [Route("{id:int}/ship", Name = "ShipSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Ship(int id)
        {
            _apiResponse.Data = await _salesOrderService.ShipAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPatch]
        [Route("{id:int}/deliver", Name = "DeliverSalesOrder")]
        [Authorize(Policy = "ManageSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Deliver(int id)
        {
            _apiResponse.Data = await _salesOrderService.DeliverAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}", Name = "CancelSalesOrder")]
        [Authorize(Policy = "DeleteSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Cancel(int id)
        {
            _apiResponse.Data = await _salesOrderService.CancelAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("search")]
        [Authorize(Policy = "ViewSalesOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Search(
            [FromQuery] int? status, 
            [FromQuery] int? customerId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            _apiResponse.Data = await _salesOrderService.SearchAsync(status, customerId, startDate, endDate);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }
    }
}
