

namespace Pal.Model
{
    public class User
    {

        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserImg { get; set; }

        public User() { }

        public User(string email) {
            Email = email;
        }

        public User(string email, string userName)
        {
            Email = email;
            UserName = userName;
            UserImg = "blank_profile_picture_640.png";
        }
    }
}
