using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pal.Model
{
    public class GroupChatRoom : ChatRoom
    {
        public string Admin { get; set; }
        public ObservableCollection<User> _Users { get; set; }

        public GroupChatRoom(string roomID, string roomTitle,string admin, ObservableCollection<User> Users,bool isDestruct) : base(roomID,roomTitle,isDestruct) {
            Admin = admin;
            _Users = Users;

        }

    }
}
