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
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [Authorize(Policy= "ViewCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<CategoryDto>>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(new APIResponse<IEnumerable<CategoryDto>>(result, "Categories fetched successfully"));
        }

        [HttpGet]
        [Route("{id:int}",Name="GetCategoryById")]
        [Authorize(Policy = "ViewCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<CategoryDto>>> GetCategoryById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(new APIResponse<CategoryDto>(result, "Category fetched successfully"));
        }

        [HttpPost]
        [Route("CreateCategory")]
        [Authorize(Policy = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtRoute("GetCategoryById", new { id = result.Id }, new APIResponse<CategoryDto>(result, "Category created successfully"));
        }

        [HttpPut]
        [Route("{id:int}",Name ="UpdateCategory")]
        [Authorize(Policy = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> UpdateCategory(int id,[FromBody] UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id,dto);
            return Ok(new APIResponse<bool>(result, "Category updated successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteCategory")]
        [Authorize(Policy = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "Category deleted successfully"));
        }


    }
}
