using Application.Common;
using Application.DTOs.Product;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Domain.Common;
using Application.Interfaces.Products;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<PaginatedResult<ProductDto>>>> GetAllProducts([FromQuery] PaginationParams @params)
        {
            var result = await _productService.GetAllAsync(@params);
            return Ok(new APIResponse<PaginatedResult<ProductDto>>(result, "Products fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}",Name="GetProductById")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<ProductDto>>> GetProductById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(new APIResponse<ProductDto>(product, "Product fetched successfully"));
        }

        [HttpGet]
        [Route("sku/{sku}", Name = "GetProductBySku")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<ProductDto>>> GetProductBySku(string sku)
        {
            var product = await _productService.GetBySkuAsync(sku);
            return Ok(new APIResponse<ProductDto>(product, "Product fetched successfully"));
        }

        [HttpGet]
        [Route("search", Name = "SearchProducts")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDto>>>> SearchProducts([FromQuery] string? name, [FromQuery] int? categoryId, [FromQuery] int? supplierId)
        {
            var products = await _productService.SearchAsync(name, categoryId, supplierId);
            return Ok(new APIResponse<IEnumerable<ProductDto>>(products, "Products fetched successfully"));
        }

        [HttpPost]
        [Route("CreateProduct")]
        [Authorize(Policy = "ManageProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtRoute("GetProductById", new { id = result.Id }, new APIResponse<ProductDto>(result, "Product created successfully"));
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
        public async Task<ActionResult<APIResponse<bool>>> UpdateProduct(int id,[FromBody] UpdateProductDto dto) 
        {
            var result = await _productService.UpdateAsync(id,dto);
            return Ok(new APIResponse<bool>(result, "Product updated successfully"));
        }

        [HttpDelete]
        [Route("{id:int}",Name= "DeleteProduct")]
        [Authorize(Policy = "DeleteProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> DeleteProduct(int id)
        {
            var result = await _productService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "Product deleted successfully"));
        }

        [HttpGet]
        [Route("low-stock")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDto>>>> GetLowStock()
        {
            var products = await _productService.GetLowStockProducts();
            return Ok(new APIResponse<IEnumerable<ProductDto>>(products, "Low stock products fetched successfully"));
        }

        [HttpGet]
        [Route("out-of-stock")]
        [Authorize(Policy = "ViewProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDto>>>> GetOutOfStock()
        {
            var products = await _productService.GetOutOfStockProducts();
            return Ok(new APIResponse<IEnumerable<ProductDto>>(products, "Out of stock products fetched successfully"));
        }

        [HttpPatch]
        [Route("{id:int}")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<ActionResult<APIResponse<string>>> PatchProduct(int id, [FromBody] JsonPatchDocument<UpdateProductDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest(new APIResponse<object>(null, "Invalid patch document") { Status = false, StatusCode = HttpStatusCode.BadRequest });

            await _productService.PatchAsync(id, patchDoc);
            return Ok(new APIResponse<string>("Product updated partially", "Patch successful"));
        }
    }
}
