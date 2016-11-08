using System.Collections.Generic;

namespace RequireClaimsInJwt.Owin
{
    public class RequireClaimsInJwtOptions
    {
        public RequireClaimsInJwtOptions()
        {
            Requirements = new List<ClaimRequirement> ();
        }

        internal List<ClaimRequirement> Requirements { get; set; }

        public void AddRequirement(ClaimRequirement req)
        {
            Requirements.Add(req);
        }
    }
}