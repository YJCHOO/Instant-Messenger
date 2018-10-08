using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Message
    {
        public string User { get; set; }
        public string Text { get; set; }
        public string Attachment { get; set; }
        public DateTime DateTime { get; set; }
    }
}
