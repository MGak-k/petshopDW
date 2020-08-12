using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEdit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int skip, int take, string filter, string sort, bool showActive)
        {
            try
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("skip", skip);
                values.Add("take", take);
                values.Add("filter", filter);
                values.Add("sort", sort);

                var hiddenColumns = new List<string>();
                var data = BLL.GetProduct(values, showActive);
                return Json(new { success = true, data, data.totalCount });
            }
            catch (Exception ex)
            {
                // Logger.ShowErrors(ex, "VehicleVehicles", User.Identity.Name, null, Request.UserHostAddress, null, null);
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult AddProduct(BusinessLogic.DAL.Product postData)
        {
            try
            {

                BLL.CreateProduct(postData);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult UpdateProduct(BusinessLogic.DAL.Product postData)
        {
            try
            {
                BLL.UpdateProduct(postData);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
}