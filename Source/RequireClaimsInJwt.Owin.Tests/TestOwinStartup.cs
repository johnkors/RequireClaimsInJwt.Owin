using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Owin;

namespace RequireClaimsInJwt.Owin.Tests
{
    public class TestOwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var opts = new RequireClaimsInJwtOptions();

            Func<IEnumerable<Claim>, bool> mustContainAtLeastOneBanana = cl => cl.Any(c => c.Type == "sub");
            var claimRequirement = new ClaimRequirement(mustContainAtLeastOneBanana, "No sub claim!");
            opts.AddRequirement(claimRequirement);
            app.UseRequireClaimsInJwt(opts);

            var testConfiguration = new HttpConfiguration();
            testConfiguration.MapHttpAttributeRoutes();
            app.UseWebApi(testConfiguration);
        }
    }
}