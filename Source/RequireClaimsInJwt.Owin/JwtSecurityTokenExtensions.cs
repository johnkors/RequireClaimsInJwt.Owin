using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace RequireClaimsInJwt.Owin
{
    internal static class JwtSecurityTokenExtensions
    {
        internal static IEnumerable<string> CheckRequirements(this JwtSecurityToken token, IEnumerable<ClaimRequirement> requirements)
        { 
            var errors = new List<string>();
            foreach (var requirement in requirements)
            {
                var fulfillsRequirement = requirement.Verify(token.Claims);
                if (!fulfillsRequirement)
                {
                    errors.Add(requirement.ErrorMsg);
                }
            }

            return errors;
        }
    }
}