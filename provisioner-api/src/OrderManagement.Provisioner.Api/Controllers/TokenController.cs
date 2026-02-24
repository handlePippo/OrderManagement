using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Provisioner.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/token")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _service;

        public TokenController(ITokenService masterService)
        {
            _service = masterService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody][Required] TokenRequestDto loginRequest, CancellationToken cancellationToken)
        {
            var jwt = await _service.GetTokenAsync(loginRequest, cancellationToken);

            if (jwt is null)
            {
                return BadRequest();
            }

            return Ok(jwt);
        }
    }
}