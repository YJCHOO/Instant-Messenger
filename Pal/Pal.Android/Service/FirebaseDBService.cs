using Xamarin.Forms;
using Pal.Service;
using Firebase.Firestore;
using System.Collections.Generic;
using Pal.Model;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseDatabaseConn))]
namespace Pal.Droid.Service
{
    class FirebaseDatabaseConn : IFirebaseDatabase
    {

        FirebaseFirestore Conn = FirebaseFirestore.GetInstance(MainActivity.app);

        public void GetMessage()
        {
            throw new System.NotImplementedException();
        }

        public void GetUser()
        {
            throw new System.NotImplementedException();
        }

        public void SetMessage()
        {
            throw new System.NotImplementedException();
        }

        public void SetUser()
        {

            

        }

        public void SetUser(string email, string name)
        {

            var NewUser = new Dictionary<string, Java.Lang.Object>
            {
                { "username", "juan" },
                { "email",email}
            };

            Conn.Collection("users")
                .Add(NewUser);


        }
    }
}