using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pal.Model
{
    public class GroupChatRoom : ChatRoom
    {
        public ObservableCollection<User> _Users { get; set; }

        public GroupChatRoom(string roomID, string roomTitle, ObservableCollection<User> Users,bool isDestruct) : base(roomID,roomTitle,isDestruct) {

            _Users = Users;

        }

    }
}
