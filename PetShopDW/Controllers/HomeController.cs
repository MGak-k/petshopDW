using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string search)
        {
            var model = BLL.GetAllProducts();
            if (search != null)
                model = model.Where(x => x.ProductName.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower())).ToList();

            return View(model);
        }
        
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}