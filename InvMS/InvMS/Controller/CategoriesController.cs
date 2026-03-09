using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        private readonly APIResponse _apiResponse;
        public CategoriesController(ICategoryService categoryService, APIResponse apiResponse)
        {
            _apiResponse = apiResponse;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [Authorize(Policy= "ViewCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllCategories()
        {
            _apiResponse.Data = await _categoryService.GetAllAsync();
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpGet]
        [Route("{id:int}",Name="GetCategoryById")]
        [Authorize(Policy = "ViewCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCategoryById(int id)
        {
            _apiResponse.Data = await _categoryService.GetByIdAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Route("CreateCategory")]
        [Authorize(Policy = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CreateCategoryDto dto)
        {

            _apiResponse.Data = await _categoryService.CreateAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            return CreatedAtRoute("GetCategoryById", new { id = ((CategoryDto)_apiResponse.Data).Id }, dto);
        }

        [HttpPut]
        [Route("{id:int}",Name ="UpdateCategory")]
        [Authorize(Policy = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateCategory(int id,[FromBody] UpdateCategoryDto dto)
        {
            _apiResponse.Data = await _categoryService.UpdateAsync(id,dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteCategory")]
        [Authorize(Policy = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
        {
            _apiResponse.Data = await _categoryService.SoftDeleteAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }


    }
}
