using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class IndividualChatRoom : ChatRoom
    {

        public User _User { get; set; }

        public IndividualChatRoom(string roomID,string roomTitle,User user,bool isDestruct):base(roomID, roomTitle,isDestruct) {

            this._User = user;

        }
    }
}
