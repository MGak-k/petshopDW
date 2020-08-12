using BusinessLogic.BusinessObjects.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace BusinessLogic.Helpers
{
    public static class Messaging
    {

        public static void GetAttachmentsForMail(out List<EmailAttachment> filesList, string Path, Guid FileId)
        {
            filesList = new List<EmailAttachment>();
            //set formats for check
            var path = new List<Extensions>()
            {
                new Extensions{Path= @Path + FileId.ToString() + ".pdf",mediaType = "application/pdf" , Extension = ".pdf"},
                new Extensions{Path= @Path + FileId.ToString() + ".png",mediaType = "image/png", Extension = ".png"},
                new Extensions{Path= @Path + FileId.ToString() + ".jpg",mediaType = "image/jpeg", Extension = ".jpg"},
                new Extensions{Path= @Path + FileId.ToString() + ".xlsx",mediaType = "application/vnd.ms-excel", Extension = ".xlsx"}
            };
            //check if file with extension above exsist in folder
            foreach (var item in path)
            {
                if (File.Exists(item.Path))
                {
                    filesList.Add(new EmailAttachment()
                    {
                        Content = File.ReadAllBytes(item.Path),
                        MediaType = item.mediaType,
                        FileName = FileId.ToString(),
                        Path = @Path,
                        IsExistingFile = true,
                        Extension = item.Extension
                    });
                }
            }
        }
    }
}
