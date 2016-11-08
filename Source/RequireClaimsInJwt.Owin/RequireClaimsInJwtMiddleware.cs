using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace RequireClaimsInJwt.Owin
{
    public class RequireClaimsInJwtMiddleware
    {
        readonly AppFunc _next;
        private readonly RequireClaimsInJwtOptions _options;
  
        public RequireClaimsInJwtMiddleware(AppFunc next, RequireClaimsInJwtOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            if (!env.IsJwtBearerTokenRequest())
            {
                await _next(env);
                return;
            }

            var bearerToken = env.GetBearerToken();
            var token = new JwtSecurityToken(bearerToken);
            var errors = token.CheckRequirements(_options.Requirements);

            if (errors.Any())
            {
                env.RespondForbiddenWith(errors);
            }
            else
            {
                await _next(env);
            }
        }
    }
}
