using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BusinessObjects.Email
{
    public class EmailAttachment
    {
        public byte[] Content { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string OldFileName { get; set; }
        public string MediaType { get; set; }
        public bool IsExistingFile { get; set; }
        public string Extension { get; set; }
        public string AzureContainer { get; set; }
        public string FileID { get; set; }

    }
}
