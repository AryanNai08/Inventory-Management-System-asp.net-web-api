using Application.Common;
using Application.DTOs.Customer;
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
        private readonly APIResponse _apiResponse;
        public CustomersController(ICustomerService customerService, APIResponse apiResponse)
        {
            _apiResponse = apiResponse;
           _customerService=customerService;
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllCustomers()
        {
            _apiResponse.Data = await _customerService.GetAllAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetCustomerById")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCustomerById(int id)
        {
            _apiResponse.Data = await _customerService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("search", Name = "SearchCustomers")]
        [Authorize(Policy = "ViewCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SearchCustomers([FromQuery] string? name, [FromQuery] string? city)
        {
            _apiResponse.Data = await _customerService.SearchAsync(name, city);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Route("CreateCustomer")]
        [Authorize(Policy = "ManageCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCustomer([FromBody] CreateCustomerDto dto)
        {

            _apiResponse.Data = await _customerService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            return CreatedAtRoute("GetCustomerById", new { id = ((CustomerDto)_apiResponse.Data).Id }, _apiResponse);
        }

        [HttpPut]
        [Route("{id:int}", Name = "UpdateCustomer")]
        [Authorize(Policy = "ManageCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            _apiResponse.Data = await _customerService.UpdateAsync(id, dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteCustomer")]
        [Authorize(Policy = "DeleteCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteCustomer(int id)
        {
            _apiResponse.Data = await _customerService.SoftDeleteAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }
    }
}
