using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace RequireClaimsInJwt.Owin
{
    public class RequireClaimsInJwtMiddleware
    {
        readonly AppFunc _next;
        private readonly RequireClaimsInJwtOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireClaimsInJwt"/> class.
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="options">The options.</param>
        public RequireClaimsInJwtMiddleware(AppFunc next, RequireClaimsInJwtOptions options)
        {
            _next = next;
            _options = options;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <returns></returns>
        public async Task Invoke(IDictionary<string, object> env)
        {
            if (!IsBearerTokenRequest(env))
            {
                await _next(env);
                return;
            }

            var headers = GetHeaders(env);
            var token = headers["Authorization"][0].Split(' ')[1];

            var errors = CheckRequirements(token).ToList();
            if (!errors.Any())
            {
                await _next(env);
                return;
            }

            env["owin.ResponseStatusCode"] = 403;
            var responseHeaders = env["owin.ResponseHeaders"] as IDictionary<string, string[]>;
            responseHeaders.Add("WWW-Authenticate", new[] { GetBearerErrorMsg(errors) });
            responseHeaders.Add("jwt-errors", new[] { string.Join(",", errors) });

            env["owin.ResponseReasonPhrase"] = "Unsatisfactory JWT";
            env["owin.ResponseHeaders"] = responseHeaders;
        }

        private static string GetBearerErrorMsg(IEnumerable<string> errors)
        {
            var strBuilder = new StringBuilder();
            foreach (var error in errors)
            {
                strBuilder.Append(error);
            }
            return string.Format("Bearer error=\"{0}\"", strBuilder);
        }

        private static bool IsBearerTokenRequest(IDictionary<string, object> env)
        {
            var headers = GetHeaders(env);

            var hasAuthorizationHeader = headers.ContainsKey("Authorization");
            if (hasAuthorizationHeader && headers["Authorization"].Length > 0)
            {
                var value = headers["Authorization"][0];
                var isBearer = value.StartsWith("Bearer", StringComparison.CurrentCultureIgnoreCase);
                var tokenString = value.Split(' ')[1];
                var hasToken = value.Split(' ').Length > 1 && !string.IsNullOrEmpty(tokenString);
                return isBearer && hasToken;
            }
            return false;
        }

        private static IDictionary<string, string[]> GetHeaders(IDictionary<string, object> env)
        {
            return env["owin.RequestHeaders"] as IDictionary<string, string[]>;
        }

        private IEnumerable<string> CheckRequirements(string encodedTokenString)
        {
            var errors = new List<string>();
            try
            {
                var token = new JwtSecurityToken(encodedTokenString);
                foreach (var requirement in _options.Requirements)
                {
                    var fulfillsRequirement = requirement.Verify(token.Claims);
                    if (!fulfillsRequirement)
                    {
                        errors.Add(requirement.ErrorMsg);
                    }
                }
            }
            catch (ArgumentException ae)
            {
                errors.Add("Token was not on a valid JWT format! " + ae.Message);
            }
            catch (Exception e)
            {
                errors.Add("An error occured while checking JWT requirements. " + e);
            }
            return errors;
        }
    }
}
