//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BusinessLogic.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailQueue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmailQueue()
        {
            this.EmailAttachments = new HashSet<EmailAttachment>();
        }
    
        public System.Guid ID { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string LastError { get; set; }
        public Nullable<System.DateTime> LastAttempt { get; set; }
        public int NoOfRetries { get; set; }
        public string EmailContent { get; set; }
        public string Subject { get; set; }
        public bool Sent { get; set; }
        public string CC { get; set; }
        public bool HasAttachment { get; set; }
        public string AttachmentExtension { get; set; }
        public string AttachmentFileName { get; set; }
        public string AttachmentFileMediaType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAttachment> EmailAttachments { get; set; }
    }
}
