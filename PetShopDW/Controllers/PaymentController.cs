using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class PaymentController : BaseController
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
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
            return RedirectToAction("Index","Home");
        }

        public ActionResult RemoveFromCart(Guid ProductID)
        {
            if (Session["cart"] != null)
            {
                var cart = (List<BusinessLogic.BusinessObjects.CartItems>)Session["cart"];
                cart.Remove(cart.Where(x => x.Product.ProductID == ProductID).FirstOrDefault());
                Session["cart"] = cart;
            }
           
            return RedirectToAction("MyCart", "Payment");
        }

        [HttpPost]
        public ActionResult ClearCart()
        {
            if (Session["cart"] != null)
            {
                Session["cart"] = null;
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyCart()
        {
            var milos = Session["cart"];
            return View((List<BusinessLogic.BusinessObjects.CartItems>)Session["cart"]);
        }

        [HttpPost]
        public JsonResult PaymentConfirm(BusinessLogic.BusinessObjects.PaymentConfirm postData)
        {
            try
            {
                var orderItems = (List<BusinessLogic.BusinessObjects.CartItems>)Session["cart"];
                var orderID = BLL.AddPayment(postData,orderItems);
                var stringOrder = orderID.ToString();
                var message = BLL.GetMessage("new-order");

                var settings = BLL.GetSettings();
                var Recipient = "milos.gak@axelyos.com";

                Dictionary<string, string> dictionaryTerms = new Dictionary<string, string>();
                dictionaryTerms.Add("[Name]", "Milos Gak");
                dictionaryTerms.Add("[OrderNumber]", stringOrder);

                var Subject = Helpers.EmailTranslator.Translate(message.Subject, dictionaryTerms);
                var Body = Helpers.EmailTranslator.Translate(message.Layout, dictionaryTerms);

                BLL.AddEmailToQueue(settings.FirstOrDefault(x => x.Active && x.Element == "sender-email")?.Value ?? "", Recipient, Body, Subject, 0, null);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
}