using Application.Common;
using Application.DTOs.Product;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<ActionResult<APIResponse>> GetAllProducts([FromQuery] PaginationParams @params)
        {
            var result = await _productService.GetAllAsync(@params);
            _apiResponse.Data = result;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
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

        [HttpGet]
        [Route("sku/{sku}", Name = "GetProductBySku")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProductBySku(string sku)
        {
            var product = await _productService.GetBySkuAsync(sku);
            _apiResponse.Data = product;
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }

        [HttpGet]
        [Route("search", Name = "SearchProducts")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SearchProducts([FromQuery] string? name, [FromQuery] int? categoryId, [FromQuery] int? supplierId)
        {
            var products = await _productService.SearchAsync(name, categoryId, supplierId);
            _apiResponse.Data = products;
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

        //[HttpPost]
        //[Route("CreateProduct")]
        //[Authorize(Policy = "ManageProducts")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> CreateProduct([FromBody] CreateProductDto dto)
        //{
        //    // 🔴 Step 1: Model Validation (MULTIPLE ERRORS)
        //    if (!ModelState.IsValid)
        //    {
        //        _apiResponse.Status = false;
        //        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //        _apiResponse.Data = null;

        //        _apiResponse.Error = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();

        //        return BadRequest(_apiResponse);
        //    }

        //    try
        //    {
        //        // 🟢 Step 2: Call Service
        //        var result = await _productService.CreateAsync(dto);

        //        _apiResponse.Data = result;
        //        _apiResponse.StatusCode = HttpStatusCode.Created;
        //        _apiResponse.Status = true;

        //        return CreatedAtRoute("GetProductById", new { id = result.Id }, _apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 🔴 Step 3: Handle unexpected errors
        //        _apiResponse.Status = false;
        //        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        _apiResponse.Data = null;
        //        _apiResponse.Error = new List<string> { ex.Message };

        //        return StatusCode(500, _apiResponse);
        //    }
        //}

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
            _apiResponse.Data = await _productService.SoftDeleteAsync(id);
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("low-stock")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLowStock()
        {
            _apiResponse.Data = await _productService.GetLowStockProducts();
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("out-of-stock")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetOutOfStock()
        {
            _apiResponse.Data = await _productService.GetOutOfStockProducts();
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPatch]
        [Route("{id:int}")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<ActionResult<APIResponse>> PatchProduct(int id, [FromBody] JsonPatchDocument<UpdateProductDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            await _productService.PatchAsync(id, patchDoc);
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = "Product updated partially";
            return Ok(_apiResponse);
        }
    }
}
