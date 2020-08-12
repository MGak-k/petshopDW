using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult AddProduct(BusinessLogic.BusinessObjects.Product product)
        {
            try
            {
                var productID = BLL.CreateProduct(product);
                return Json(new { success = true , productID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        public JsonResult AddProduct2(BusinessLogic.BusinessObjects.Product product)
        {
            try
            {
                
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

        public JsonResult Upload(Guid Id)
        {
            try
            {
                if (Request.Files.Count > 0)
                {

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Server.MapPath("/filedump/product/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        var fullpath = Path.Combine(path, Id + "_temp.png");
                        file.SaveAs(fullpath);

                        Image imgPhoto = Image.FromFile(fullpath);
                        Image tumbnailImgPhoto = Helpers.ImageHelper.FixedSize(imgPhoto, 70, 94);
                        tumbnailImgPhoto.Save(path + Id + "_thumbnail.png");
                        tumbnailImgPhoto.Dispose();

                        Image resizedImgPhoto = Helpers.ImageHelper.FixedSize(imgPhoto, 705, 940);
                        resizedImgPhoto.Save(path + Id + ".png");
                        resizedImgPhoto.Dispose();
                        imgPhoto.Dispose();

                        System.IO.File.Delete(fullpath);
                    }
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                Logger.Module = "ProductController";
                Logger.Exception(ex);
                return Json(new { Response = "Error", Result = ex.Message });
            }
        }

        public JsonResult GetDropdowns()
        {
            try
            {
                var model = BLL.GetCategoryDropdowns();
                return Json(new { Response = "OK", data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return Json(ex.Message);
            }
        }
    }
}