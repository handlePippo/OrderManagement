using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderManagement.Provisioner.Api.Application.Services
{
    public sealed class TokenService : ITokenService
    {
        private const string _jwtKey = "Jwt:Key";
        private const string _jwtIssuer = "Jwt:Issuer";
        private const string AdminEmail = "admin@demo.it";
        private const string AdminRole = "Admin";
        private const string UserRole = "User";
        private string JwtKey => _configuration[_jwtKey]!;
        private string JwtIssuer => _configuration[_jwtIssuer]!;

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TokenService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var loginRequest = _mapper.Map<TokenRequest>(loginRequestDto);

            var user = await _userRepository.GetAsync(loginRequest, cancellationToken)
                ?? throw new InvalidOperationException("Invalid credentials.");

            var keyBytes = Convert.FromHexString(JwtKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[2]
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Role, GetRole(loginRequest.Email))
            };

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new InvalidOperationException("Failed to build JWT.");
            }

            return new TokenResponseDto(jwt);
        }

        private static string GetRole(string email) => email == AdminEmail ? AdminRole : UserRole;
    }
}