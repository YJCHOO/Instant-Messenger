using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Moment
    {
        public string MomentId { get; set; }
        public string Sender { get; set; }
        public string DateTime { get; set; }
        public string Description { get; set; }
        public Attachment _Attachment { get; set; }
        public Dictionary<string, bool> Receiver { get; set; }

        public Moment(string momentId, string sender, string dateTime, string description, Attachment attachment, string isPublic, Dictionary<string, bool> receiver)
        {
            MomentId = momentId;
            Sender = sender;
            DateTime = dateTime;
            Description = description;
            _Attachment = attachment;
            Receiver = receiver;
        }

        public Moment(string sender, string description,Attachment attachment) {
            Sender = sender;
            Description = description;
            _Attachment = attachment;
        }

    }
}
