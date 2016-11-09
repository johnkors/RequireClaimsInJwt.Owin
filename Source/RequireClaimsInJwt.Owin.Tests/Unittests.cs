using System.Linq;
using System.Net;
using Microsoft.Owin.Testing;
using Xunit;

namespace RequireClaimsInJwt.Owin.Tests
{
    public class Unittests
    {
        private const string JWTWithSubClaim = @"eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL3NvbWVpc3N1ZXIuY29tIiwic3ViIjoidGhlLXVzZXItaWQiLCJuYmYiOjE0Nzg2OTczNjQsImV4cCI6MTQ3ODcwMDk2NCwiaWF0IjoxNDc4Njk3MzY0fQ.anything";
        private const string JWTWithoutSubClaim = @"eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL3NvbWVpc3N1ZXIuY29tIiwibmJmIjoxNDc4Njk3NTAwLCJleHAiOjE0Nzg3MDExMDAsImlhdCI6MTQ3ODY5NzUwMH0.anything";

        [Fact]
        public async void IgnoresRequestsMissingAuthorizationHeader()
        {
            using (var server = TestServer.Create<TestOwinStartup>())
            {
                var request = server.CreateRequest("/"); // .AddHeader("Authorization", $"Bearer {JWTWithoutSubClaim}");
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("Bearer")]
        [InlineData("Bearer ")]
        [InlineData("Bearer  ")]
        [InlineData("Bearer this.isinvalid.jwt")]
        public async void IgnoresBearerTokenRequestsHavingInvalidJwts(string bearerTokenValue)
        {
            using (var server = TestServer.Create<TestOwinStartup>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", string.Format("{0}",bearerTokenValue));
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async void ComplainsAboutMissingSubClaimWhenSubClaimIsMissing()
        {
            using (var server = TestServer.Create<TestOwinStartup>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", string.Format("Bearer {0}", JWTWithoutSubClaim));
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
                Assert.Equal("Unsatisfactory JWT", response.Headers.WwwAuthenticate.ToString());

                var customHeader = response.Headers.First( h => h.Key == "jwt-errors").Value;
                Assert.Equal("No sub claim!", string.Join(",", customHeader));
            }
        }

        [Fact]
        public async void DoesNotComplainsAboutMissingSubClaimWhenSubClaimExists()
        {
            using (var server = TestServer.Create<TestOwinStartup>())
            {
                var request = server.CreateRequest("/").AddHeader("Authorization", string.Format("Bearer {0}", JWTWithSubClaim));
                var response = await request.GetAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
