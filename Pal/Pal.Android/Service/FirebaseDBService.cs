using Xamarin.Forms;
using Pal.Service;
using Firebase.Firestore;
using System.Collections.Generic;
using Android.Gms.Tasks;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Java.Lang;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseDatabaseConn))]
namespace Pal.Droid.Service
{
    class FirebaseDatabaseConn : Java.Lang.Object ,IFirebaseDatabase, IOnCompleteListener,IOnSuccessListener,IOnFailureListener
    {

        private FirebaseFirestore Conn = FirebaseFirestore.GetInstance(MainActivity.app);
        private const string usercollection = "users";
        private string username { set; get; }

        public void GetMessage()
        {
            throw new System.NotImplementedException();
        }

        public void SetMessage()
        {
            throw new System.NotImplementedException();
        }


        public void SetUser(string email, string name)
        {  
            var NewUser = new Dictionary<string, Java.Lang.Object>
            {
                { "username", "" },
            };
            Conn.Collection("users").Document(email).Set(NewUser).AddOnSuccessListener(this);
        }

        public void GetUser(string TempEmail)
        {
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);
            Path.Get().AddOnCompleteListener(this);
            
        }

        public void UpdateUser(string TempEmail,string TempUsername) {
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);
            Path.Update("username", TempUsername)
            .AddOnSuccessListener(this)
            .AddOnFailureListener(this);
        }




        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                DocumentSnapshot temp = (DocumentSnapshot)task.Result;
                if (temp.Exists())
                {
                    var TempDictionary = temp.Data;
                    username = (string)TempDictionary["username"];
                    UserSetting.UserName=username;
                }
                else {
                    username = null;
                }
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Debug.Write("Failure Listener need your attention");
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Debug.Write("OnSuccess Listener invoked.");
        }
    }
}