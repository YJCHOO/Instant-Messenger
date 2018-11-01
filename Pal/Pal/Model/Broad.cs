using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Broad
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Poll _Poll { get; set; }
        public User _User { get; set; }
        public DateTime PostedDateTime { get; set; }
        
    }
}
