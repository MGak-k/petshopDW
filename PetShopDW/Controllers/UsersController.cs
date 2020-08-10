using PetShopDW.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class UsersController : BaseController
    {
        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //[Authorize(Roles = "Admin,User,Entry")]
        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Admin,User,Entry")]
        [HttpPost]
        public JsonResult Get(int skip, int take, string filter, string sort, bool showInactive)
        {
            try
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("skip", skip);
                values.Add("take", take);
                values.Add("filter", filter);
                values.Add("sort", sort);

                var hiddenColumns = new List<string>();

                var data = BLL.GetUsers(values, showInactive);

                return Json(new { success = true, data, data.totalCount });
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return Json(new { success = false });
            }
        }

        //[Authorize(Roles = "Admin,User,Entry")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                ViewBag.Mode = "Edit";
                var model = BLL.GetUserProfileById(id);
                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return null;
            }
        }


        //[Authorize(Roles = "Admin,User,Entry")]
        public JsonResult Upload(Guid Id)
        {
            try
            {
                if (Request.Files.Count > 0)
                {

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var path = Server.MapPath("/filedump/users/");

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

                        //BLL.UpdateLocationPhoto(Id);
                        System.IO.File.Delete(fullpath);
                    }
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return Json(new { Response = "Error", Result = ex.Message });
            }
        }



        [Authorize(Roles = "Admin,User,Entry")]
        public JsonResult GetDropdowns()
        {
            try
            {
                var model = BLL.GetUsersDropdowns();
                return Json(new { Response = "OK", data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return Json(ex.Message);
            }
        }
        [Authorize(Roles = "Admin,Userapprove")]
        public JsonResult GetUserGroups()
        {
            try
            {
                var model = BLL.GetUserGroupsDropdowns();
                return Json(new { Response = "OK", data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Module = "UsersController";
                Logger.Exception(ex);
                return Json(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult DeleteUser(Guid UserID)
        {
            try
            {
                BLL.DeleteUser(UserID, CurrentUser.ID);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult Restore(Guid UserID)
        {
            try
            {
                BLL.RestoreUser(UserID, CurrentUser.ID);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

    }
}