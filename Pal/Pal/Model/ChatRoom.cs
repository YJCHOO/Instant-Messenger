
using System;
using System.Collections.ObjectModel;

namespace Pal.Model
{
    public class ChatRoom
    {
        public string RoomTilte { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public string Image { get; set; }
        public string LastMsg { get; set; }

        public ChatRoom()
        {
        }

        public ChatRoom(string roomTilte, ObservableCollection<User> users)
        {
            RoomTilte = roomTilte;
            Users = users;
        }
    }
}
