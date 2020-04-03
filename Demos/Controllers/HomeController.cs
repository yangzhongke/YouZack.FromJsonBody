using Demos.Models;
using Microsoft.AspNetCore.Mvc;
using YouZack.FromJsonBody;

namespace Demos.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Test([FromJsonBody]string phoneNumber, [FromJsonBody]string test1, 
            [FromJsonBody]int? age, [FromJsonBody] bool gender, 
            [FromJsonBody] double salary,[FromJsonBody]DirectionTypes dir)
        {
            return Json($"phoneNumber={phoneNumber},test1={test1},age={age},gender={gender},salary={salary},dir={dir}");
        }
    }
}
