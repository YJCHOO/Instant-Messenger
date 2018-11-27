using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pal.Model
{
    public class Poll
    {
        public string PollId { get; set; }
        public string RoomId { get; set; }
        public string Title { get; set; }
        public bool IsClose { get; set;}
        public ObservableCollection<string> _Option { get; set; }
        public Dictionary<string, string> Result { get; set; }

        public Poll() { }

        public Poll(string pollId, string roomId, string title, bool isClose, ObservableCollection<string> Option, Dictionary<string, string> result)
        {
            PollId = pollId;
            RoomId = roomId;
            Title = title;
            IsClose = isClose;
            _Option = Option;
            Result = result;
        }
    }
}
