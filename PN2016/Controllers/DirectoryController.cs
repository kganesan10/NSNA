using System.Web.Mvc;

namespace PN2016.Controllers
{
    public class DirectoryController : Controller
    {
        // GET: Directory
        public ActionResult Index()
        {
            return View();
        }

        // GET: Directory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Directory/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}