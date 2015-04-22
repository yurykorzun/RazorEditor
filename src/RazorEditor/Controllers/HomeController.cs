using System.Web.Mvc;

namespace RazorEditor.Controllers
{
    public class HomeController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}