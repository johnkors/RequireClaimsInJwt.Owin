using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace RequireClaimsInJwt.Owin.Tests
{
    public class SomeApiController : ApiController
    {
        [Route("")]
        public OkResult Get()
        {
            return Ok();
        }
    }
}
