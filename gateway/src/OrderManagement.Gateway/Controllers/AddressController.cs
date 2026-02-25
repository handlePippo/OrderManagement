using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly IProvisionerApiAddressClient _client;
        public AddressController(IProvisionerApiAddressClient client)
        {
            _client = client;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IReadOnlyList<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<AddressDto>>> ListAsync(CancellationToken token)
        {
            var dto = await _client.ListAsync(token);

            return Ok(dto ?? Array.Empty<AddressDto>()!);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddressDto>> GetAsync([FromRoute] int id, CancellationToken token)
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
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AddressDto>> CreateAsync([FromBody][Required] CreateAddressDto request, CancellationToken token)
        {
            await _client.CreateAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateAddressDto request, CancellationToken token)
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