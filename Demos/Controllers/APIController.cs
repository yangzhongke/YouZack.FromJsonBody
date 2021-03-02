using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<int> Post([FromJsonBody("i1")] int i3, [FromJsonBody] int i2,
            [FromJsonBody("author.age")]int aAge,[FromJsonBody("author.father.name")] string dadName)
        {
            Debug.WriteLine(aAge);
            Debug.WriteLine(dadName);
            return i3 + i2+aAge;
        }
    }
}
