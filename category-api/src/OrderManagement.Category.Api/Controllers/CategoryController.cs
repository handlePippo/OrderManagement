using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Category.Api.Application.DTOs;
using OrderManagement.Category.Api.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Category.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _service.ListAsync(token);

            return Ok(dto ?? Array.Empty<CategoryDto>()!);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> GetAsync([FromRoute] int id, CancellationToken token)
        {
            var dto = await _service.GetByIdAsync(id, token);

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("exists/{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ExistsAsync([FromRoute] int id, CancellationToken token)
        {
            var exists = await _service.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> AddAsync([FromBody][Required] CreateCategoryDto request, CancellationToken token)
        {
            await _service.AddAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateCategoryDto request, CancellationToken token)
        {
            await _service.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
        {
            await _service.DeleteAsync(id, token);

            return NoContent();
        }
    }
}