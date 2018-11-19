using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Board
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Attachment _Attachment { get; set; }
        public User _User { get; set; }
        public DateTime PostedDateTime { get; set; }

        public Board(string title, string description, Attachment Attachment, User User, DateTime postedDateTime)
        {
            Title = title;
            Description = description;
            _Attachment = Attachment;
            _User = User;
            PostedDateTime = postedDateTime;
        }
    }
}
