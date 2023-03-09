using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    ViewBag.ErrorMessage = "Unauthorized";
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Sorry, The resource you requested was not found!";
                    break;
                default:
                    ViewBag.ErrorMessage = $"Something went wrong, Error Code {statusCode}";
                    break;
            }

            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            ViewBag.ErrorMessage = "Something went wrong, Please contact us.";

            return View("Index");
        }
    }
}
