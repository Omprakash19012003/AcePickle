using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using acepickle_chat_api.Helper;

namespace acepickle_chat_api.Helper
{
    public class JwtTokenService : IJwtTokenService
    {

        public JwtTokenService()
        {

        }
        // public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        // {
        //     var tokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidateAudience = false,
        //         ValidateIssuer = false,
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SkillerSecretKey@54321")),
        //         ValidateLifetime = false
        //     };

        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     SecurityToken securityToken;
        //     var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //     var jwtSecurityToken = securityToken as JwtSecurityToken;
        //     if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //         throw new SecurityTokenException("Invalid token");

        //     return principal;
        // }
    }
}
