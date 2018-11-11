using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Attachment
    {

        public string FileName { get; set; }
        public string AttachmentUri { get; set; }


        public Attachment() { }

        public Attachment(string fileName, string attachmentUri)
        {
            FileName = fileName;
            AttachmentUri = attachmentUri;
        }
    }
}
