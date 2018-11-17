
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Pal.Model
{
    public class ChatRoom
    {
        public string RoomID { get; set; }
        public string Image { get; set; }
        public string RoomTilte { get; set; }
        public bool IsDestruct { get; set; }

        public ChatRoom()
        {
            Image = "blank_profile_picture_640.png";
        }

        public ChatRoom(string roomID,string roomTilte,bool isDestruct)
        {
            RoomID = roomID;
            RoomTilte = roomTilte;
            Image = "blank_profile_picture_640.png";
            IsDestruct = isDestruct;
        }

        public ChatRoom(string roomID, string roomTilte, bool isDestruct,DateTime lastUpdate)
        {
            RoomID = roomID;
            RoomTilte = roomTilte;
            Image = "blank_profile_picture_640.png";
            IsDestruct = isDestruct;
        }
    }
}
