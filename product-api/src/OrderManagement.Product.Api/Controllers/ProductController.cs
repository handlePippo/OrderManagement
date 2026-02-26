using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Application.Interfaces;
using OrderManagement.Product.Api.Configuration;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Product.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IStockService _stockService;

        public ProductController(IProductService service, IStockService stockService)
        {
            _service = service;
            _stockService = stockService;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _service.ListAsync(token);

            return Ok(dto ?? Array.Empty<ProductDto>()!);
        }

        [HttpPost("range")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetRangeAsync([FromBody][Required] GetProductRangeDto requestDto, CancellationToken token)
        {
            var dto = await _service.GetRangeAsync(requestDto, token);

            return Ok(dto ?? Array.Empty<ProductDto>()!);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetAsync([FromRoute] int id, CancellationToken token)
        {
            var dto = await _service.GetAsync(id, token);

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
            var exists = await _service.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> AddAsync([FromBody][Required] CreateProductDto request, CancellationToken token)
        {
            await _service.CreateAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateProductDto request, CancellationToken token)
        {
            await _service.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpPut("{id:int}/stock/increase/{qty:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IncreaseStockAsync([FromRoute] int id, [FromRoute] int qty, CancellationToken token)
        {
            await _stockService.IncreaseStock(id, qty, token);

            return Accepted();
        }

        [HttpPut("{id:int}/stock/decrease/{qty:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DecreaseStockAsync([FromRoute] int id, [FromRoute] int qty, CancellationToken token)
        {
            await _stockService.DecreaseStock(id, qty, token);

            return Accepted();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
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