using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class User
    {
        public string UserName { get; set; }
        public string UserImg { get; set; }

        public User(string userName)
        {
            UserName = userName;
        }

        public User(string userName, string userImg)
        {
            UserName = userName;
            UserImg = userImg;
        }
    }
}
