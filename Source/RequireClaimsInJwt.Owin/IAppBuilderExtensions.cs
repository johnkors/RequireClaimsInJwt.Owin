using Owin;

namespace RequireClaimsInJwt.Owin
{
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseRequireClaimsInJwt(this IAppBuilder appBuilder, RequireClaimsInJwtOptions opts)
        {
            appBuilder.Use<RequireClaimsInJwtMiddleware>(opts);
            return appBuilder;
        }
    }
}