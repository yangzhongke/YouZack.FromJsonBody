using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouZack.FromJsonBody;

namespace Demos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {

        [HttpPost]
        [HttpGet]
        public async Task<int> Post([FromJsonBody("i2")] int i1, [FromJsonBody] int i2)
        {
            return i1 + i2;
        }
    }
}
