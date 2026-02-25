using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Gateway.Application.DTOs.Products;
using OrderManagement.Gateway.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductApiClient _client;
        public ProductController(IProductApiClient client)
        {
            _client = client;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _client.ListAsync(token);

            return Ok(dto ?? Array.Empty<ProductDto>()!);
        }

        [HttpPost("range")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetRangeAsync([FromBody][Required] GetProductRangeDto requestDto, CancellationToken token)
        {
            var dto = await _client.GetRangeAsync(requestDto, token);

            return Ok(dto ?? Array.Empty<ProductDto>()!);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetAsync([FromRoute] int id, CancellationToken token)
        {
            var dto = await _client.GetAsync(id, token);

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("exists/{id:int}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ExistsAsync([FromRoute] int id, CancellationToken token)
        {
            var exists = await _client.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> AddAsync([FromBody][Required] CreateProductDto request, CancellationToken token)
        {
            await _client.CreateAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateProductDto request, CancellationToken token)
        {
            await _client.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
        {
            await _client.DeleteAsync(id, token);

            return NoContent();
        }
    }
}