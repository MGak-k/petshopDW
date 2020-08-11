﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Helpers;

namespace BusinessLogic
{
   public class PanelLogic : IDisposable
    {
        protected DAL.praksaPSEntities DB;
        protected DAL.User CurrentUser;
        public PanelLogic()
        {
            DB = new DAL.praksaPSEntities();
        }

        public void Dispose()
        {
            if (DB != null)
                DB.Dispose();
        }

        #region User
        public BusinessObjects.ListWrapper GetUsers(Dictionary<string, object> options, bool showInactive)
        {
            var data = DB.Users
                  .AsNoTracking()
                  .OrderByDescending(x => x.FirstName)
                  .AsQueryable();


            if (showInactive == true)
                data = data.Where(x => !x.Active).AsQueryable();
            else
            {
                data = data.Where(x => x.Active).AsQueryable();
            }
            var querying = QueryingHelper<DAL.User>.Filter(data, options, typeof(BusinessObjects.Users));
            data = querying.Results;

            var usersViewModel = data
                         .AsEnumerable()
                         .Select(x => new BusinessObjects.Users()
                         {
                             ID = x.ID,
                             UserName = x.UserName,
                             FirstName = x.FirstName,
                             LastName = x.LastName,
                             Active = x.Active,
                             Email = x.Email
                         })
                         .AsQueryable();

            return new BusinessLogic.BusinessObjects.ListWrapper()
            {
                data = usersViewModel,
                totalCount = querying.TotalCount
            };

        }

        public BusinessObjects.UserDropdowns GetUsersDropdowns()
        {

            return new BusinessObjects.UserDropdowns()
            {

                Users = (from t in DB.Users
                         where t.Active
                         orderby t.FirstName
                         select new BusinessObjects.GenericDropdown()
                         {
                             id = t.ID,
                             text = t.FirstName + " " + t.LastName
                         }).ToList(),

            };
        }
        public List<DAL.UserGroup> GetAllGroups()
        {
            return DB.UserGroups.Where(x => x.Active).ToList();
        }
      
        public BusinessObjects.UserDropdowns GetUserGroupsDropdowns()
        {

            return new BusinessObjects.UserDropdowns()
            {

                UserGroups = (from t in DB.UserGroups
                              where t.Active
                              select new BusinessObjects.Generic()
                              {
                                  id = t.ID,
                                  text = t.Name
                              }).ToList(),

            };
        }

        public void DeleteUser(Guid UserID, Guid CurrentUser)
        {
            var user = DB.Users.FirstOrDefault(x => x.ID == UserID);

            if (user != null)
            {
                user.Active = false;
                user.UpdatedBy = CurrentUser;
                user.UpdatedDate = DateTime.Now;
                DB.SaveChanges();
            }
        }

        public void RestoreUser(Guid UserID, Guid CurrentUser)
        {
            var user = DB.Users.FirstOrDefault(x => x.ID == UserID);

            if (user != null)
            {
                user.Active = true;
                user.UpdatedBy = CurrentUser;
                user.UpdatedDate = DateTime.Now;
                DB.SaveChanges();
            }
        }

        public DAL.User AddToUserTable(string firstname, string lastname, string username, string email, bool active)
        {
            using (var dbContextTransaction = DB.Database.BeginTransaction())
            {
                try
                {

                    var user = new DAL.User();

                    user.ID = Guid.NewGuid();
                    user.FirstName = firstname;
                    user.LastName = lastname;
                    user.UserName = username;
                    user.Email = email;
                    user.UpdatedBy = user.ID;
                    user.UpdatedDate = DateTime.Now;
                    user.Active = active;

                    DB.Users.Add(user);
                    DB.SaveChanges();

                    dbContextTransaction.Commit();
                    return user;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return null;
                }
            }
        }

        public DAL.User GetUserByUserName(string UserName)
        {
            var user = new DAL.User();
            try
            {
                user = DB.Users.FirstOrDefault(x => string.Compare(x.UserName, UserName, true, CultureInfo.InvariantCulture) == 0);
            }
            catch (ArgumentNullException)
            {
            }
            return user ?? new DAL.User();
        }

        public DAL.User GetUserByAccount(string account)
        {
            var user = new DAL.User();
            try
            {
                user = DB.Users.FirstOrDefault(x => string.Compare(x.Email, account, true, CultureInfo.InvariantCulture) == 0);
            }
            catch (ArgumentNullException)
            {
            }
            return user ?? new DAL.User();
        }

        public BusinessObjects.Users GetUserProfileById(Guid UserID)
        {
            var user = DB.Users.FirstOrDefault(x => x.ID == UserID);

            var model = new BusinessObjects.Users()
            {
                ID = user.ID,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Active = user.Active,
            };

            return model;
        }

        public void SetCurrentUser(string userName)
            {
                CurrentUser = DB.Users.FirstOrDefault(x => x.UserName == userName);
            }

        public List<DAL.UserGroup> GetRoleNamesByID(int[] roleID)
        {
            var userGroups = new List<DAL.UserGroup>();

            foreach (var item in roleID)
            {
                var data = DB.UserGroups.FirstOrDefault(x => x.ID == item);
                userGroups.Add(data);
            }
            return userGroups;
        }


       
        public List<DAL.Setting> GetSettings()
        {
            return DB.Settings.Where(x => x.Active).ToList();
        }

        public DAL.EmailTemplate GetMessage(string code)
        {
            return DB.EmailTemplates.FirstOrDefault(x => x.Code == code);
        }

        public Guid AddEmailToQueue(string Sender, string Recipient, string Content, string Subject, int offset, List<BusinessObjects.Email.EmailAttachment> Files = null, List<string> CCs = null)
        {
            Logger logger = new Logger("Email Queue ADD");

            logger.Verbose("User Offset", offset, "");

            if (Recipient != "")
            {
                CCs = CCs ?? new List<string>();
                CCs.Add(DB.Settings.Where(z => z.Element == "heather-mail").FirstOrDefault().Value);
                string strCC = null;
                if (CCs.Count() > 0)
                    strCC = string.Join(",", CCs.ToArray());

                DAL.EmailQueue email = new DAL.EmailQueue
                {
                    ID = Guid.NewGuid(),
                    SenderEmail = Sender,
                    RecipientEmail = Recipient,
                    EmailContent = Content,
                    Subject = Subject,
                    TimeStamp = DateTime.Now.AddMinutes(offset),
                    NoOfRetries = 0,
                    Sent = false,
                    CC = strCC
                };

                if (Files != null && Files.Count() > 0)
                {
                    email.HasAttachment = true;
                    foreach (var f in Files)
                    {
                        var att = new DAL.EmailAttachment()
                        {
                            ID = Guid.NewGuid(),
                            EmailQueueID = email.ID,
                            Extension = f.Extension,
                            Filename = f.FileName,
                            Path = f.Path,
                            FileMediaType = f.MediaType,
                            TimeStamp = DateTime.Now.AddMinutes(offset),
                            IsExistingFile = f.IsExistingFile,
                            AzureContainer = f.AzureContainer,
                            FileID = f.FileID
                        };

                        if (!f.IsExistingFile)
                        {
                            att.Path = $"{f.Path}\\{email.ID}.{System.IO.Path.GetExtension(f.FileName).Replace(".", "")}";
                            BusinessLogic.Helpers.FileHelper.ByteArrayToFile(att.Path, f.Content);
                        }
                        DB.EmailAttachments.Add(att);
                    }
                }

                try
                {
                    DB.EmailQueues.Add(email);
                    DB.SaveChanges();
                    return email.ID;
                }
                catch (Exception ex)
                {
                    return Guid.Empty;
                }
            }
            else
                return Guid.Empty;
        }


        public static void SendRegisterEmail(string userName, int offset)
        {
            using (BusinessLogic.PanelLogic bll = new BusinessLogic.PanelLogic())
            {
                try
                {
                    var message = bll.GetMessage("register-account-en");

                    var user = bll.GetUserByUserName(userName);
                    var settings = bll.GetSettings();

                    var Recipient = user.Email;

                    Dictionary<string, string> dictionaryTerms = new Dictionary<string, string>();
                    dictionaryTerms.Add("[Name]", user.FirstName + " " + user.LastName);
                    dictionaryTerms.Add("[LoginName]", user.UserName);
                    dictionaryTerms.Add("[Link]", settings.FirstOrDefault(x => x.Active && x.Element == "local-url")?.Value ?? "");
                    //dictionaryTerms.Add("[ResetLink]", body);

                    var Subject = PetShopDW.Helpers.EmailTranslator.Translate(message.Subject, dictionaryTerms);
                    var Body = PetShopDW.Helpers.EmailTranslator.Translate(message.Layout, dictionaryTerms);
                    var sender = settings.FirstOrDefault(x => x.Active && x.Element == "sender-email")?.Value ?? "";

                    bll.AddEmailToQueue(sender, Recipient, Body, Subject, offset, null);

                    BusinessLogic.PanelLogic.SendRegisterEmailToAdmin(user.UserName, offset);

                }
                catch (Exception ex)
                {
                    //Logger.Module = "Messaging.cs";
                    //Logger.Exception(ex);
                }
            }
        }

        public static void SendRegisterEmailToAdmin(string userName, int offset)
        {
            using (BusinessLogic.PanelLogic bll = new PanelLogic())
            {
                try
                {

                    var message = bll.GetMessage("register-request-en");
                    var settings = bll.GetSettings();
                    var user = bll.GetUserByUserName(userName);

                    Dictionary<string, string> dictionaryTerms = new Dictionary<string, string>();
                    dictionaryTerms.Add("[Name]", user.FirstName);
                    dictionaryTerms.Add("[LoginName]", user.FirstName + " " + user.LastName);
                    dictionaryTerms.Add("[Link]", settings.FirstOrDefault(x => x.Active && x.Element == "local-url")?.Value ?? "");

                    var Subject = PetShopDW.Helpers.EmailTranslator.Translate(message.Subject, dictionaryTerms);
                    var Body = PetShopDW.Helpers.EmailTranslator.Translate(message.Layout, dictionaryTerms);
                    var sender = settings.FirstOrDefault(x => x.Active && x.Element == "sender-email")?.Value ?? "";

                    // TODO: GET ADMIN EMAIL AND SEND EMAIL TO ADMIN
                    //foreach (var item in bll.get(LocationID))
                    //{
                    //    bll.AddEmailToQueue(sender, item, Body, Subject, offset, null);
                    //}



                }
                catch (Exception ex)
                {
                    //Logger.Module = "Messaging.cs";
                    //Logger.Exception(ex);
                }
            }
        }
        #endregion

        #region Category

        public BusinessObjects.ListWrapper GetCategory(Dictionary<string, object> options, bool showActive)
        {
            var data = DB.Categories
                  .OrderByDescending(x => x.CategoryName)
                  .AsQueryable();

            if (showActive == true)
                data = data.Where(x => x.IsActive.Value).AsQueryable();
            else
            {
                data = data.Where(x => !x.IsActive.Value).AsQueryable();
            }

            var querying = QueryingHelper<DAL.Category>.Filter(data, options, typeof(BusinessObjects.Category));
            data = querying.Results;

            var categoryViewModel = data
                         .AsEnumerable()
                         .Select(x => new BusinessObjects.Category()
                         {
                             CategoryID = x.CategoryID,
                             CategoryName = x.CategoryName,
                             IsActive = x.IsActive,
                             IsDeleted = x.IsDeleted
                         })
                         .AsQueryable();

            return new BusinessLogic.BusinessObjects.ListWrapper()
            {
                data = categoryViewModel,
                totalCount = querying.TotalCount
            };

        }

        public void CreateCategory(DAL.Category postdata)
        {
            var category = new DAL.Category();
            if (category != null)
            {
                category.CategoryID = Guid.NewGuid();
                category.CategoryName = postdata.CategoryName;
                category.IsActive = true;
                category.IsDeleted = false;

                DB.Categories.Add(category);
                DB.SaveChanges();
            }
        }

        public void UpdateCategory(DAL.Category postData)
        {
            var category = DB.Categories.FirstOrDefault(x => x.CategoryID == postData.CategoryID);
            if (category != null)
            {
                category.CategoryName = postData.CategoryName;
                category.IsActive = postData.IsActive;
                category.IsDeleted = postData.IsDeleted;

                DB.SaveChanges();
            }
        }



        #endregion

        #region Product

        public BusinessObjects.ListWrapper GetProduct(Dictionary<string, object> options, bool showActive)
        {
            var data = DB.Products
                  .OrderByDescending(x => x.ProductName)
                  .AsQueryable();

            if (showActive == true)
                data = data.Where(x => x.IsActive.Value).AsQueryable();
            else
            {
                data = data.Where(x => !x.IsActive.Value).AsQueryable();
            }

            var querying = QueryingHelper<DAL.Product>.Filter(data, options, typeof(BusinessObjects.Product));
            data = querying.Results;

            var productViewModel = data
                         .AsEnumerable()
                         .Select(x => new BusinessObjects.Product()
                         {
                             ProductID = x.ProductID,
                             ProductName = x.ProductName,
                             CategoryID = x.CategoryID,
                             Description = x.Description,
                             CreatedDate = x.CreatedDate,
                             Image = x.Image,
                             Price = x.Price,
                             ProductCount = x.ProductCount,
                             UpdatedDate = x.UpdatedDate,
                             IsActive = x.IsActive,
                             IsDeleted = x.IsDeleted

                         })
                         .AsQueryable();

            return new BusinessLogic.BusinessObjects.ListWrapper()
            {
                data = productViewModel,
                totalCount = querying.TotalCount
            };

        }

        public BusinessObjects.Product GetProductByID(Guid ProductID)
        {
            var data = DB.Products.Where(x => x.ProductID == ProductID)
                  .OrderByDescending(x => x.ProductName)
                  .Select(z => new BusinessObjects.Product()
                  {
                      ProductID = z.ProductID,
                      ProductName = z.ProductName,
                      CategoryID = z.CategoryID,
                      Description = z.Description,
                      CreatedDate = z.CreatedDate,
                      Image = z.Image,
                      Price = z.Price,
                      ProductCount = z.ProductCount,
                      UpdatedDate = z.UpdatedDate,
                      IsActive = z.IsActive,
                  }).FirstOrDefault();

            return data;

        }

        public List<DAL.Product> GetAllProducts()
        {
            var data = DB.Products
                  .OrderByDescending(x => x.ProductName)
                  .ToList();

            return data;
        }
        public void CreateProduct(DAL.Product postData)
        {
            var product = new DAL.Product();
            if (product != null)
            {
                product.ProductID = Guid.NewGuid();
                product.ProductName = postData.ProductName;
                product.CategoryID = postData.CategoryID;
                product.Description = postData.Description;
                product.CreatedDate = postData.CreatedDate;
                product.Image = postData.Image;
                product.Price = postData.Price;
                product.ProductCount = postData.ProductCount;
                product.UpdatedDate = postData.UpdatedDate;
                product.IsActive = postData.IsActive;
                product.IsDeleted = postData.IsDeleted;

                DB.Products.Add(product);
                DB.SaveChanges();
            }
        }

        public void UpdateProduct(DAL.Product postData)
        {
            var product = DB.Products.FirstOrDefault(x => x.ProductID == postData.ProductID);
            if (product != null)
            {
                product.ProductName = postData.ProductName;
                product.CategoryID = postData.CategoryID;
                product.Description = postData.Description;
                product.CreatedDate = postData.CreatedDate;
                product.Image = postData.Image;
                product.Price = postData.Price;
                product.ProductCount = postData.ProductCount;
                product.UpdatedDate = postData.UpdatedDate;
                product.IsActive = postData.IsActive;
                product.IsDeleted = postData.IsDeleted;

                DB.SaveChanges();
            }
        }



        #endregion



    }
}
