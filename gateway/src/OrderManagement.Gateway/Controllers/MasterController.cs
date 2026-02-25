using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Token;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/master")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class MasterController : ControllerBase
    {
        private readonly IProvisionerApiMasterClient _client;

        public MasterController(IProvisionerApiMasterClient client)
        {
            _client = client;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody][Required] TokenRequestDto loginRequest, CancellationToken cancellationToken)
        {
            var jwt = await _client.GetTokenAsync(loginRequest, cancellationToken);

            if (jwt is null)
            {
                return BadRequest();
            }

            return Ok(jwt);
        }

        [HttpPost("createUser")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> CreateAsync([FromBody][Required] CreateUserDto request, CancellationToken token)
        {
            await _client.CreateUserAsync(request, token);

            return Created();
        }
    }
}