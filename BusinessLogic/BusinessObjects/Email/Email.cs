using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects.Email
{
    public class Email
    {
        public Guid ID { get; set; }
        public string TimeStamp { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string LastError { get; set; }
        public string LastAttempt { get; set; }
        public int NoOfRetries { get; set; }
        public string EmailContent { get; set; }
        public string Subject { get; set; }
        public bool Sent { get; set; }
        public string Cc { get; set; }

        public bool HasAttachment { get; set; }
        public int NumOfAttachments { get; set; }
    }
}
