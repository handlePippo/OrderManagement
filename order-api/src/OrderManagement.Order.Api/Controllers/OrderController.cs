using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Application.DTOs.Orders.Update;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Configuration;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _service.ListAsync(token);

            return Ok(dto ?? Array.Empty<OrderDto>()!);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateAuthorizationFilter))]
        public async Task<ActionResult<OrderDto>> GetAsync([FromRoute] int id, CancellationToken token)
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
        [ServiceFilter(typeof(ValidateAuthorizationFilter))]
        public async Task<ActionResult<bool>> ExistsAsync([FromRoute] int id, CancellationToken token)
        {
            var exists = await _service.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateAuthorizationFilter))]
        public async Task<ActionResult<OrderDto>> CreateAsync([FromBody][Required] CreateOrderDto request, CancellationToken token)
        {
            await _service.CreateAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateAuthorizationFilter))]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateOrderDto request, CancellationToken token)
        {
            await _service.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateAuthorizationFilter))]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
        {
            await _service.DeleteAsync(id, token);

            return NoContent();
        }
    }
}