using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Gateway.Application.DTOs.Orders;
using OrderManagement.Gateway.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/orders")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApiClient _client;
        public OrderController(IOrderApiClient client)
        {
            _client = client;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _client.ListAsync(token);

            return Ok(dto ?? Array.Empty<OrderDto>()!);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetAsync([FromRoute] Guid id, CancellationToken token)
        {
            var dto = await _client.GetAsync(id, token);

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("exists/{id:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ExistsAsync([FromRoute] Guid id, CancellationToken token)
        {
            var exists = await _client.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> CreateAsync([FromBody][Required] CreateOrderDto request, CancellationToken token)
        {
            await _client.CreateAsync(request, token);

            return Created();
        }

        [HttpPost("submit")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> SubmitAsync([FromRoute] Guid id, CancellationToken token)
        {
            await _client.SubmitAsync(id, token);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody][Required] UpdateOrderDto request, CancellationToken token)
        {
            await _client.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpDelete("submitted/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSubmittedAsync([FromRoute] Guid id, CancellationToken token)
        {
            await _client.DeleteSubmittedAsync(id, token);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token)
        {
            await _client.DeleteAsync(id, token);

            return NoContent();
        }
    }
}