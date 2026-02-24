using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Configuration;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Provisioner.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }


        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> ListAsync(CancellationToken token)
        {
            var dto = await _service.ListAsync(token);

            if (dto is null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateUserAuthorizationFilter))]
        public async Task<ActionResult<UserDto>> GetAsync([FromRoute] int id, CancellationToken token)
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
        [ServiceFilter(typeof(ValidateUserAuthorizationFilter))]
        public async Task<ActionResult<bool>> ExistsAsync([FromRoute] int id, CancellationToken token)
        {
            var exists = await _service.ExistsAsync(id, token);

            return Ok(exists);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> CreateAsync([FromBody][Required] CreateUserDto request, CancellationToken token)
        {
            await _service.CreateAsync(request, token);

            return Created();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateUserAuthorizationFilter))]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody][Required] UpdateUserDto request, CancellationToken token)
        {
            await _service.UpdateAsync(id, request, token);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateUserAuthorizationFilter))]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
        {
            await _service.DeleteAsync(id, token);

            return NoContent();
        }
    }
}