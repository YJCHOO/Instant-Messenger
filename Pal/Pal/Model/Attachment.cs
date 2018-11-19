using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Attachment
    {

        public string FileName { get; set; }
        public string Thumbnail{ get; set; }
        public string AttachmentUri { get; set; }
        public FileData _FileData { get; set; }


        public Attachment() { }

        public Attachment(string fileName, string attachmentUri)
        {
            FileName = fileName;
            AttachmentUri = attachmentUri;
        }

        public Attachment(string fileName, string thumbnail, FileData FileData) : this(fileName, thumbnail)
        {
            _FileData = FileData;
        }
    }
}
