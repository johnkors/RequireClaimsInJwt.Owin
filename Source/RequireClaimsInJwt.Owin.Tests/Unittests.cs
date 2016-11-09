using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Owin;
using Xunit;

namespace RequireClaimsInJwt.Owin.Tests
{
    public class Unittests
    {
        private const string JWTWithSubClaim = @"eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL3NvbWVpc3N1ZXIuY29tIiwic3ViIjoidGhlLXVzZXItaWQiLCJuYmYiOjE0Nzg2OTczNjQsImV4cCI6MTQ3ODcwMDk2NCwiaWF0IjoxNDc4Njk3MzY0fQ.anything";
        private const string JWTWithoutSubClaim = @"eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL3NvbWVpc3N1ZXIuY29tIiwibmJmIjoxNDc4Njk3NTAwLCJleHAiOjE0Nzg3MDExMDAsImlhdCI6MTQ3ODY5NzUwMH0.anything";
        
        [Theory]
        [InlineData("Bearer")]
        [InlineData("Bearer ")]
        [InlineData("Bearer  ")]
        [InlineData("Bearer this.isinvalid.jwt")]
        public async void DoesNothingForInvalidJwts(string bearerTokenValue)
        {
            using (var server = TestServer.Create<StartupJwtMustHaveSub>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", $"{bearerTokenValue}");
                var response = await request.GetAsync();

                Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [Fact]
        public async void ComplainsAboutMissingSubClaim()
        {
            using (var server = TestServer.Create<StartupJwtMustHaveSub>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", $"Bearer {JWTWithoutSubClaim}");
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
                Assert.Equal("Unsatisfactory JWT", response.Headers.WwwAuthenticate.ToString());

                var customHeader = response.Headers.First( h => h.Key == "jwt-errors").Value;
                Assert.Equal("No sub claim!", string.Join(",", customHeader));
            }
        }

        [Fact]
        public async void DoesNotComplainsAboutMissingSubClaim()
        {
            using (var server = TestServer.Create<StartupJwtMustHaveSub>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", $"Bearer {JWTWithSubClaim}");
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }


    }


    public class StartupJwtMustHaveSub
    {public void Configuration(IAppBuilder app)
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
