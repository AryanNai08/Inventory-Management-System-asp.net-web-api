using Application.Common;
using Application.DTOs.Product;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly APIResponse _apiResponse;
        
        public ProductsController(IProductService productService,APIResponse apiResponse)
        {
            _productService = productService;
            _apiResponse = apiResponse;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllProducts()
        {
            var products=await _productService.GetAllAsync();
            _apiResponse.Data=products;
            _apiResponse.Status = true;
            _apiResponse.StatusCode=HttpStatusCode.OK;
            return _apiResponse;
        }

        [HttpGet]
        [Route("{id:int}",Name="GetProductById")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProductById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            _apiResponse.Data = product;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }

        [HttpPost]
        [Route("CreateProduct")]
        [Authorize(Policy = "ManageProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateProduct([FromBody] CreateProductDto dto)
        {
            _apiResponse.Data = await _productService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            return CreatedAtRoute("GetProductById", new { id = ((ProductDto)_apiResponse.Data).Id }, _apiResponse);
        }

        [HttpPut]
        [Route("{id:int}",Name ="UpdateProduct")]
        [Authorize(Policy = "ManageProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id,[FromBody] UpdateProductDto dto) 
        {
            _apiResponse.Data = await _productService.UpdateAsync(id,dto);
            _apiResponse.Status=true;
            _apiResponse.StatusCode=HttpStatusCode.OK;

            return _apiResponse;
        }

        [HttpDelete]
        [Route("{id:int}",Name= "DeleteProduct")]
        [Authorize(Policy = "DeleteProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
        {
            _apiResponse.Data=await _productService.SoftDeleteAsync(id);
            _apiResponse.Status=true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }

    }
}
