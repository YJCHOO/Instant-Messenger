using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class Board
    {
        public string BoardMessageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Attachment _Attachment { get; set; }
        public User _User { get; set; }
        public DateTime PostedDateTime { get; set; }

        public Board(string boardMessageId, string title, string description, Attachment Attachment, User User, DateTime postedDateTime)
        {
            BoardMessageId = boardMessageId;
            Title = title;
            Description = description;
            _Attachment = Attachment;
            _User = User;
            PostedDateTime = postedDateTime;
        }

        public Board()
        {
        }
    }
}
