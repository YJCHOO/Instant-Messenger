using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    class Moments
    {
        public string MomentId { get; set; }
        public string Sender { get; set; }
        public string DateTime { get; set; }
        public string Description { get; set; }
        public string AttachmentUri { get; set; }
        public string IsPublic { get; set; }
        public Dictionary<string, bool> Receiver { get; set; }

        public Moments(string momentId, string sender, string dateTime, string description, string attachmentUri, string isPublic, Dictionary<string, bool> receiver)
        {
            MomentId = momentId;
            Sender = sender;
            DateTime = dateTime;
            Description = description;
            AttachmentUri = attachmentUri;
            IsPublic = isPublic;
            Receiver = receiver;
        }
    }
}
