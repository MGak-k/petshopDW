using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class HomeController : BaseController
    {
        //[Authorize(Roles = "Admin,User")]
        public ActionResult Index(string search)
        {

          var model = BLL.GetAllProducts();
            if (search != null)
                model = model.Where(x => x.ProductName.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower())).ToList();


            return View(model);
        }
       
        public ActionResult AddToCart(Guid ProductID)
        {

            
            if (Session["cart"] == null)
            {
                var cart = new List<BusinessLogic.BusinessObjects.CartItems>();
                var product = BLL.GetProductByID(ProductID);
                cart.Add(new BusinessLogic.BusinessObjects.CartItems()
                {
                    Product = product,
                    Quantity = 1

                });
                Session["cart"] = cart;
                Session["count"] = 1;
            }
            else
            {
                List<BusinessLogic.BusinessObjects.CartItems> cart = (List<BusinessLogic.BusinessObjects.CartItems>)Session["cart"];
                var product = BLL.GetProductByID(ProductID);
                cart.Add(new BusinessLogic.BusinessObjects.CartItems()
                {
                    Product = product,
                    Quantity = 1

                });
                Session["cart"] = cart;
                Session["count"] = cart.Count();


            }
            return RedirectToAction("Index");
        }

        public ActionResult MyCart()
        {
            var milos = Session["cart"];
            return View((List<BusinessLogic.BusinessObjects.CartItems>)Session["cart"]);

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}