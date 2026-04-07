using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.DTOs.Supplier;
using Application.DTOs.Product;
using Application.DTOs.PurchaseOrder;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Domain.Common;
using Application.Interfaces.Suppliers;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [Route("GetAllSuppliers")]
        [Authorize(Policy = "ViewSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PaginatedResult<SupplierDto>>>> GetAllSuppliers([FromQuery] PaginationParams @params)
        {
            var result = await _supplierService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<SupplierDto>>(result, "Suppliers fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetSupplierById")]
        [Authorize(Policy = "ViewSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<SupplierDto>>> GetSupplierById(int id)
        {
            var result = await _supplierService.GetByIdAsync(id);
            return Ok(new APIResponse<SupplierDto>(result, "Supplier fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}/products", Name = "GetProductsBySupplierId")]
        [Authorize(Policy = "ViewSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<ProductDto>>>> GetProductsBySupplierId(int id)
        {
            var result = await _supplierService.GetProductsBySupplierIdAsync(id);
            return Ok(new APIResponse<List<ProductDto>>(result, "Supplier products fetched successfully"));
        }

        [HttpPost]
        [Route("CreateSupplier")]
        [Authorize(Policy = "ManageSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<SupplierDto>>> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var result = await _supplierService.CreateAsync(dto);
            return CreatedAtRoute("GetSupplierById", new { id = result.Id }, new APIResponse<SupplierDto>(result, "Supplier created successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateSupplier")]
        [Authorize(Policy = "ManageSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> UpdateSupplier(int id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await _supplierService.UpdateAsync(id, dto);
            return Ok(new APIResponse<bool>(result, "Supplier updated successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteSupplier")]
        [Authorize(Policy = "DeleteSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> DeleteSupplier(int id)
        {
            var result = await _supplierService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "Supplier deleted successfully"));
        }

        [HttpGet]
        [Route("{id:int}/purchase-orders", Name = "GetPurchaseOrdersBySupplierId")]
        [Authorize(Policy = "ViewSuppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<PurchaseOrderDto>>>> Getpurchaseorders(int id)
        {
            var result = await _supplierService.GetPurchaseOrdersBySupplierId(id);
            return Ok(new APIResponse<List<PurchaseOrderDto>>(result, "Purchase orders fetched successfully"));
        }
    }
}
