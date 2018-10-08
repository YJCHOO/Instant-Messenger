
using System;
using System.Collections.ObjectModel;

namespace Pal.Model
{
    public class ChatRoom
    {
        public String RoomTilte { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public String Image { get; set; }

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
