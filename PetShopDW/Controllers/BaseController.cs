using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class BaseController : Controller
    {
        protected BusinessLogic.PanelLogic BLL;
        protected BusinessLogic.Helpers.Logger Logger = new BusinessLogic.Helpers.Logger();
        protected BusinessLogic.DAL.User CurrentUser;

        public BaseController()
        {
            BLL = new BusinessLogic.PanelLogic();
        }

        protected override void Dispose(bool disposing)
        {
            if (BLL != null)
                //BLL.Dispose();

            base.Dispose(disposing);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new Newtonsoft.JsonResult.JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
      
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = CurrentUser = BLL.GetUserByUserName(User.Identity.Name);
            }
            base.OnActionExecuting(filterContext);
        }

        public Boolean IsUserAdmin()
        {
            return User.IsInRole("Admin") || User.IsInRole("company");
        }

        public string ShowErrors(Exception ex)
        {
            StringBuilder errorDetails = new StringBuilder();

            if (ex is DbEntityValidationException)
            {
                foreach (var validationErrors in ((DbEntityValidationException)ex).EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorDetails.AppendLine(string.Format("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage));
                    }
                }
            }

            var innerException = ex;
            int counter = 0;

            while (innerException != null && counter < 10)
            {
                errorDetails.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
                counter++;
            }

            return errorDetails.ToString();
        }

        public string FormatModelStateErrors(ViewDataDictionary Data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ModelState modelState in Data.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    sb.Append(error.ErrorMessage + "<br />");
                }
            }
            return sb.ToString();
        }
      
    }
}