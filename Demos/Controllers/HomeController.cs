using Demos.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            [FromJsonBody][Range(0,100,ErrorMessage ="Age must be between 0 and 100")]int? age, 
            [FromJsonBody] bool gender, 
            [FromJsonBody] double salary,[FromJsonBody]DirectionTypes dir,
            [FromJsonBody][Required]string name)
        {
            if(ModelState.IsValid==false)
            {
                var errors = ModelState.SelectMany(e => e.Value.Errors).Select(e=>e.ErrorMessage);
                return Json("Invalid input!"+string.Join("\r\n",errors));
            }
            return Json($"phoneNumber={phoneNumber},test1={test1},age={age},gender={gender},salary={salary},dir={dir}");
        }
    }
}
