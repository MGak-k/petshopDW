using BusinessLogic.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShopDW.HangfireJobs
{
    public class EmailJob
    {
        private static readonly string _lock = "LockHangfire";

        public static void Execute()
        {
            Logger logger = new Logger("Email Queue");

            try
            {
                lock (_lock)
                {
                    ConcurrentBag<BusinessLogic.DAL.EmailQueue> emailQueue;
                    try
                    {
                        using (BusinessLogic.PanelLogic bll = new BusinessLogic.PanelLogic())
                        {
                            emailQueue = new ConcurrentBag<BusinessLogic.DAL.EmailQueue>(bll.GetEmailsForSending().ToList());

                            logger.Verbose($"Processing email queue with {emailQueue.Count} items", "", "");

                            foreach (var e in emailQueue)
                            {
                                EmailHelper eh = new EmailHelper();
                                eh.Subject = e.Subject;
                                eh.Recipient = e.RecipientEmail;
                                eh.Body = e.EmailContent;


                                if (!string.IsNullOrEmpty(e.CC))
                                    eh.CCs = e.CC;
                                
                                try
                                {
                                    eh.Send();
                                    e.Sent = true;
                                    logger.Verbose($"Email queue item sent from {e.SenderEmail} to {eh.Recipient} with subject {eh.Subject}", "", "");
                                }
                                catch (Exception ex)
                                {
                                    e.NoOfRetries++;
                                    e.LastAttempt = DateTime.Now;
                                    e.LastError = ex.ToString();

                                    logger.Exception(ex);

                                    if (e.NoOfRetries == 5)
                                    {
                                        /// send e-mail to admin
                                        eh.Subject = "Error sending e-mail";
                                        eh.Recipient = "milos.gak@axelyos.com";
                                        eh.CCs = "milos.gak@axelyos.com";
                                        eh.Body = "<p>E-mail ID: " + e.ID.ToString() + "</p><p>E-mail error: " + e.LastError + "</p>";
                                        try
                                        {
                                            eh.Send();
                                        }
                                        catch
                                        {
                                            logger.Exception(ex);
                                        }
                                    }
                                }
                                bll.UpdateEmailInQueue(e);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Exception(ex);
                    }
                    finally
                    {
                        logger.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
            }
            finally
            {
                logger.Close();
            }
        }
    }
}