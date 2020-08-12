using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShopDW.Controllers
{
    public class EmailQueueController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int skip, int take, string filter, string sort, bool showSent)
        {
            try
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("skip", skip);
                values.Add("take", take);
                values.Add("filter", filter);
                values.Add("sort", sort);

                var hiddenColumns = new List<string>();

                var data = BLL.GetEmailsInQueue(values, User.Identity.Name, showSent);
                return Json(new { success = true, data, data.totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }


        public JsonResult Resend(Guid ID)
        {
            try
            {
                BusinessLogic.Helpers.EmailHelper eh = new BusinessLogic.Helpers.EmailHelper();

                var email = BLL.GetEmailFromQueue(ID);
                if (email == null)
                    throw new Exception($"Email ID '{ID}' doesn't exists!");

                eh.Subject = email.Subject;
                eh.Body = email.EmailContent;
                eh.Recipient = email.RecipientEmail;

                //Attachments for mail         
                var FileId = ID;
                var Path = @"C:\Project\PetShop\PetShopDW\PetShopDW\filedump" + FileId + @"\";
                var Files = new List<BusinessLogic.BusinessObjects.Email.EmailAttachment>();
                BusinessLogic.Helpers.Messaging.GetAttachmentsForMail(out Files, Path, FileId);
                eh.Attachemnts = Files;

                try
                {
                    eh.Send();
                    email.Sent = true;
                }
                catch (Exception ex)
                {
                    Logger.Module = "EmailQueueController";
                    Logger.Exception(ex);

                    email.NoOfRetries++;
                    email.LastAttempt = DateTime.Now;
                    email.LastError = ex.ToString();

                    if (email.NoOfRetries == 5)
                    {
                        /// send e-mail to admin
                        eh.Subject = "Error sending e-mail";
                        eh.Recipient = "gakm08@gmail.com";
                        if (!string.IsNullOrEmpty(email.SenderEmail))
                            eh._From = email.SenderEmail;
                        eh.Body = "<p>E-mail ID: " + email.ID.ToString() + "</p><p>E-mail error: " + email.LastError + "</p>";

                        try
                        {
                            eh.Send();
                        }
                        catch
                        {
                        }
                    }

                }

                BLL.UpdateEmailInQueue(email);

                return Json(new { Response = "OK", Result = "" });
            }
            catch (Exception ex)
            {
                Logger.Module = "EmailQueueController";
                Logger.Exception(ex);
                return Json(new { Response = "Error", Result = ex.Message });
            }

        }

    }
}