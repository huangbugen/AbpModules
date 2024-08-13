using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Abp.AspNet.JwtBearer
{
    public class JwtCreator
    {
        public static string CreateToken(TokenCreateModel tokenCreateModel, params Claim[] claims)
        {
            var claimList = claims?.ToList() ?? new List<Claim>();
            claimList.Add(new Claim("userId", tokenCreateModel.UserId));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenCreateModel.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenCreateModel.Issuer,
                audience: tokenCreateModel.Audience,
                claims: claimList.ToArray(),
                expires: DateTime.Now.Add(tokenCreateModel.Ts),
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
    }
}