﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration )
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.DisplayName),
                new Claim(ClaimTypes.Email,user.Email)
            };


            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // SECURITY KEY 
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:AuthKey"] ?? string.Empty)); // lw rg3 null 5leh string fadi 

            var token = new JwtSecurityToken(
                audience: _configuration["JWT:ValidAudience"],
                issuer: _configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"] ?? "0")),// lw rg3 null 7to b zero
                claims:authClaims,
                signingCredentials:new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
                );
          return  new JwtSecurityTokenHandler().WriteToken(token);
                
                }    /*   claims:authClaims,*/
    }
}
