using Application.Common;
using Application.DTOs.Customer;
using Application.DTOs.SalesOrder;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PaginatedResult<CustomerDto>>>> GetAllCustomers([FromQuery] PaginationParams @params)
        {
            var result = await _customerService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<CustomerDto>>(result, "Customers fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetCustomerById")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<CustomerDto>>> GetCustomerById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return Ok(new APIResponse<CustomerDto>(result, "Customer fetched successfully"));
        }

        [HttpGet]
        [Route("search", Name = "SearchCustomers")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<CustomerDto>>>> SearchCustomers([FromQuery] string? name, [FromQuery] string? city)
        {
            var result = await _customerService.SearchAsync(name, city);
            return Ok(new APIResponse<List<CustomerDto>>(result, "Customers fetched successfully"));
        }

        [HttpPost]
        [Route("CreateCustomer")]
        [Authorize(Policy = "ManageCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<CustomerDto>>> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var result = await _customerService.CreateAsync(dto);
            return CreatedAtRoute("GetCustomerById", new { id = result.Id }, new APIResponse<CustomerDto>(result, "Customer created successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateCustomer")]
        [Authorize(Policy = "ManageCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            var result = await _customerService.UpdateAsync(id, dto);
            return Ok(new APIResponse<bool>(result, "Customer updated successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteCustomer")]
        [Authorize(Policy = "DeleteCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> DeleteCustomer(int id)
        {
            var result = await _customerService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "Customer deleted successfully"));
        }

        [HttpGet]
        [Route("{id:int}/sales-orders", Name = "GetSalesOrdersByCustomerId")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<List<SalesOrderDto>>>> GetSalesorder(int id)
        {
            var result = await _customerService.GetSalesOrdersByCustomerIdAsync(id);
            return Ok(new APIResponse<List<SalesOrderDto>>(result, "Sales orders fetched successfully"));
        }
    }
}
