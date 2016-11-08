using System.Collections.Generic;
using System.Linq;

namespace RequireClaimsInJwt.Owin
{
    public class RequireClaimsInJwtOptions
    {
        public RequireClaimsInJwtOptions()
        {
            var containsClaimsRequirement = new ClaimRequirement(c => c.Any(), "No claims present in JWT!");
            Requirements = new List<ClaimRequirement> { containsClaimsRequirement };
        }

        public List<ClaimRequirement> Requirements { get; set; }
    }
}