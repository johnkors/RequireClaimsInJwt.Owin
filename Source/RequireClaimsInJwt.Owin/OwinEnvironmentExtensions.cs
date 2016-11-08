using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace RequireClaimsInJwt.Owin
{
    internal static class OwinEnvironmentExtensions
    {

        internal static bool IsJwtBearerTokenRequest(this IDictionary<string, object> env)
        {
            var headers = GetHeaders(env);

            var hasAuthorizationHeader = headers.ContainsKey("Authorization");
            if (hasAuthorizationHeader && headers["Authorization"].Length > 0)
            {
                var value = headers["Authorization"][0];
                var isBearer = value.StartsWith("Bearer", StringComparison.CurrentCultureIgnoreCase);
                var tokenString = value.Split(' ')[1];
                var hasToken = value.Split(' ').Length > 1 && !string.IsNullOrEmpty(tokenString);
                if (isBearer && hasToken)
                {
                    return IsJwt(tokenString);
                }
            }
            return false;
        }

        internal static string GetBearerToken(this IDictionary<string, object> env)
        {
            var headers = env.GetHeaders();
            return headers["Authorization"][0].Split(' ')[1];
        }

        internal static void RespondForbiddenWith(this IDictionary<string, object> env, IEnumerable<string> errors)
        {
            env["owin.ResponseStatusCode"] = 403;
            var responseHeaders = env["owin.ResponseHeaders"] as IDictionary<string, string[]>;
            responseHeaders.Add("WWW-Authenticate", new[] { "Unsatisfactory JWT" });
            responseHeaders.Add("jwt-errors", new[] { string.Join(",", errors) });

            env["owin.ResponseReasonPhrase"] = "Unsatisfactory JWT";
            env["owin.ResponseHeaders"] = responseHeaders;
        }

        private static IDictionary<string, string[]> GetHeaders(this IDictionary<string, object> env)
        {
            return env["owin.RequestHeaders"] as IDictionary<string, string[]>;
        }

        private static bool IsJwt(string tokenString)
        {
            try
            {
                // ctor throws ArgumentException if not on JWT format
                new JwtSecurityToken(tokenString);
                return true;
            }
            catch (ArgumentException ae)
            {
                return false;
            }
        }
    }
}