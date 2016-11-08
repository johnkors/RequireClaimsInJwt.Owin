using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RequireClaimsInJwt.Owin
{
    public class ClaimRequirement
    {
        public ClaimRequirement(Func<IEnumerable<Claim>, bool> requirement, string errorMsgIfRequirementReturnsFalse)
        {
            Verify = requirement;
            ErrorMsg = errorMsgIfRequirementReturnsFalse;
        }
        public Func<IEnumerable<Claim>, bool> Verify { get; }
        public string ErrorMsg { get; }
    }
}