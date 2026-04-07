using Application.Common;
using Application.DTOs.PurchaseOrder;
using Domain.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Interfaces.PurchaseOrders;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        [Route("GetAllOrders")]
        [Authorize(Policy = "ViewPurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PaginatedResult<PurchaseOrderDto>>>> GetAllOrders([FromQuery] PaginationParams @params)
        {
            var result = await _purchaseOrderService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<PurchaseOrderDto>>(result, "Purchase orders fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetOrderById")]
        [Authorize(Policy = "ViewPurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PurchaseOrderDto>>> GetOrderById(int id)
        {
            var result = await _purchaseOrderService.GetByIdAsync(id);
            return Ok(new APIResponse<PurchaseOrderDto>(result, "Purchase order fetched successfully"));
        }

        [HttpPost]
        [Route("CreateOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PurchaseOrderDto>>> CreateOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            var result = await _purchaseOrderService.CreateAsync(dto);
            return CreatedAtRoute("GetOrderById", new { id = result.Id }, new APIResponse<PurchaseOrderDto>(result, "Purchase order created successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PurchaseOrderDto>>> UpdateOrder(int id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            var result = await _purchaseOrderService.UpdateAsync(id, dto);
            return Ok(new APIResponse<PurchaseOrderDto>(result, "Purchase order updated successfully"));
        }

        [HttpPut]
        [Route("{id:int}/approve", Name = "ApproveOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> ApproveOrder(int id)
        {
            var result = await _purchaseOrderService.ApproveAsync(id);
            return Ok(new APIResponse<bool>(result, "Purchase order approved successfully"));
        }

        [HttpPut]
        [Route("{id:int}/receive", Name = "ReceiveOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> ReceiveOrder(int id)
        {
            var result = await _purchaseOrderService.ReceiveAsync(id);
            return Ok(new APIResponse<bool>(result, "Purchase order received successfully"));
        }

        [HttpPut]
        [Route("{id:int}/cancel", Name = "CancelOrder")]
        [Authorize(Policy = "DeletePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> CancelOrder(int id)
        {
            var result = await _purchaseOrderService.CancelAsync(id);
            return Ok(new APIResponse<bool>(result, "Purchase order canceled successfully"));
        }
    }
}
