using Application.Common;
using Application.DTOs.PurchaseOrder;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly APIResponse _apiResponse;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, APIResponse apiResponse)
        {
            _purchaseOrderService = purchaseOrderService;
            _apiResponse = apiResponse;
        }

        [HttpGet]
        [Route("GetAllOrders")]
        [Authorize(Policy = "ViewPurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllOrders()
        {
            _apiResponse.Data = await _purchaseOrderService.GetAllAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetOrderById")]
        [Authorize(Policy = "ViewPurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetOrderById(int id)
        {
            _apiResponse.Data = await _purchaseOrderService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPost]
        [Route("CreateOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            _apiResponse.Data = await _purchaseOrderService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            return CreatedAtRoute("GetOrderById", new { id = ((PurchaseOrderDto)_apiResponse.Data).Id }, _apiResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateOrder(int id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            _apiResponse.Data = await _purchaseOrderService.UpdateAsync(id, dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPut]
        [Route("{id:int}/approve", Name = "ApproveOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ApproveOrder(int id)
        {
            _apiResponse.Data = await _purchaseOrderService.ApproveAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPut]
        [Route("{id:int}/receive", Name = "ReceiveOrder")]
        [Authorize(Policy = "ManagePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ReceiveOrder(int id)
        {
            _apiResponse.Data = await _purchaseOrderService.ReceiveAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }

        [HttpPut]
        [Route("{id:int}/cancel", Name = "CancelOrder")]
        [Authorize(Policy = "DeletePurchaseOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CancelOrder(int id)
        {
            _apiResponse.Data = await _purchaseOrderService.CancelAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }
    }
}
