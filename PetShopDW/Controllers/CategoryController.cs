using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Category
        public ActionResult Index()
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
                var data = BLL.GetCategory(values, showActive);
                return Json(new { success = true, data, data.totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult AddCategory(BusinessLogic.DAL.Category postData)
        {
            try { 

                BLL.CreateCategory(postData);
            return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult UpdateCategory(BusinessLogic.DAL.Category postData)
        {
            try { 
                BLL.UpdateCategory(postData);
            return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
}