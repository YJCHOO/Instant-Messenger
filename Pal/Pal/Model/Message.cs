using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Message
    {

        public string MessageId { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public string AttachmentUri { get; set; }
        public string AttachmentFileName { get; set; }
        public DateTime SendDateTime { get; set; }
        public bool IsDestruct { get; set; }
        public Dictionary<string, bool> IsRead { get; set;}

        public Message() { }

        public Message(string senderEmail, string senderUser, string text) {
            this.SenderEmail = senderEmail;
            this.SenderName = senderUser;
            this.Text = text;
        }


        public Message(string senderEmail, string senderUser, string text,string attachment,string attachmentFileName)
        {
            this.SenderEmail = senderEmail;
            this.SenderName = senderUser;
            this.Text = text;
            this.AttachmentUri = attachment;
            this.AttachmentFileName = attachmentFileName;
        }

        public Message(string id,string senderEmail, string senderName, string text, string attachment,string attachmentFileName, DateTime sendDateTime,bool isDestruct,Dictionary<string,bool> isRead)
        {
            MessageId = id;
            SenderEmail = senderEmail;
            SenderName = senderName;
            Text = text;
            AttachmentUri = attachment;
            AttachmentFileName = attachmentFileName;
            SendDateTime = sendDateTime;
            IsDestruct = isDestruct;
            IsRead = isRead;
        }
    }
}
