using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace APIOkta.Controllers
{
    [Produces("application/json")]
    [Route("api/Token")]
    [Authorize]
    public class TokenController : Controller
    {
        // GET: api/Token
        [HttpGet]
        public string Get()
        {
            string exp = User.Claims.Where(c => c.Type == "exp").First().Value;

            return "Your token is valid until " + (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(Double.Parse(exp)).ToString();
        }
    }
}
