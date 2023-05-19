using HumanResourcesDepartment.Models.DTO;
using HumanResourcesDepartment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HumanResourcesDepartment.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginDTO> _validation;
        public AuthService(UserManager<ApplicationUser> userManager, IValidator<LoginDTO> validation, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _validation = validation;
        }

        public IResult LoginUser(LoginDTO loginDTO)
        {
            // Validacija loginDTO
            var validationResult = _validation.ValidateAsync(loginDTO).GetAwaiter().GetResult();
            if (!validationResult.IsValid)
            {
                return Results.BadRequest();
            }

            // Provera korisničkih podataka i generisanje tokena
            var user = _userManager.FindByNameAsync(loginDTO.Username).GetAwaiter().GetResult();
            if (user != null && _userManager.CheckPasswordAsync(user, loginDTO.Password).GetAwaiter().GetResult())
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Issuer"],
                    audience: _configuration["Audience"],
                    expires: DateTime.Now.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Results.Ok(new TokenDTO()
                {
                    Username = user.UserName,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                });
            }

            return Results.Unauthorized();
        }
    }
}
