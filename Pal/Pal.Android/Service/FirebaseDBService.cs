using Xamarin.Forms;
using Pal.Service;
using Firebase.Firestore;
using System.Collections.Generic;
using Android.Gms.Tasks;
using System.Diagnostics;
using Pal.Model;
using Pal.Droid.EventListeners;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System;
using System.Globalization;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseDatabaseConn))]
namespace Pal.Droid.Service
{
    class FirebaseDatabaseConn : IFirebaseDatabase
    {

        private const string usercollection = "users";
        private const string friendsListCollection = "friendsList";

        private FirebaseFirestore Conn = FirebaseFirestore.GetInstance(MainActivity.app);
        private IListenerRegistration ReatimeListener = null;
        ObservableCollection<Message> messages = new ObservableCollection<Message>();
        ObservableCollection<object> Rooms = new ObservableCollection<object>();

        public Task<bool> DestructMessage(String MessagesId) {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
                    Conn.Collection("messages").Document(MessagesId).Update("UserRead." + UserSetting.UserEmail.Replace(".", ":"), true)
                        .AddOnSuccessListener(new OnSuccessListener((Java.Lang.Object obj) => {
                            ResultCompletionSource.SetResult(true);
                        })).AddOnFailureListener(new OnFailureListener((Java.Lang.Exception e)=> {
                            ResultCompletionSource.SetResult(false);
                        }));
            return ResultCompletionSource.Task;
        }

        public Task<string> SetRoomDestruct(string roomId,bool destructStatus) {
            TaskCompletionSource<string> ResultCompletionSource = new TaskCompletionSource<string>();
            Conn.Collection("roomList").Document(roomId).Update("isDestruct", destructStatus)
                .AddOnSuccessListener(new OnSuccessListener((Java.Lang.Object obj) =>
                {
                    ResultCompletionSource.SetResult(null);

                }))
                .AddOnFailureListener(new OnFailureListener((Java.Lang.Exception e)=>{
                    ResultCompletionSource.SetResult(e.Message);
            }));
            return ResultCompletionSource.Task;
        }

        public Task<IndividualChatRoom> SearchIndividualChatRoom(string userEmail) {

            TaskCompletionSource<IndividualChatRoom> ResultCompletionSource = new TaskCompletionSource<IndividualChatRoom>();
            IndividualChatRoom individualChat = null;
            Query query = Conn.Collection("roomList")
                .WhereEqualTo("users." + userEmail.Replace(".",":"), true)
                .WhereEqualTo("users." + UserSetting.UserEmail.Replace(".",":"), true)
                .WhereEqualTo("isGroup",false);

            query.Get().AddOnCompleteListener(new OnCompleteEventHandleListener(async (Android.Gms.Tasks.Task obj) =>
            {
                if (obj.IsSuccessful)
                {
                    QuerySnapshot TempDocSnapshot = (QuerySnapshot)obj.Result;
                    if (!TempDocSnapshot.IsEmpty)
                    {
                        foreach (DocumentSnapshot documentSnapshot in TempDocSnapshot.Documents)
                        {
                            var tempUser = (Android.Runtime.JavaDictionary)documentSnapshot.Data["users"];
                            {
                                var id = documentSnapshot.Id;
                                
                                User user = new User();
                                var isDestruct = (bool)documentSnapshot.Data["isDestruct"];

                                foreach (string useremail in tempUser.Keys)
                                {
                                    if (string.Compare(useremail, UserSetting.UserEmail.Replace(".", ":")) != 0)
                                    {
                                        user.Email = useremail;
                                    }
                                }
                                user.UserName = await GetUsername(user.Email.Replace(":","."));
                                individualChat = new IndividualChatRoom(id, null, user, isDestruct);
                            }
                        }
                        ResultCompletionSource.SetResult(individualChat);
                    }
                    else
                    {
                        ResultCompletionSource.SetResult(null);
                    }
                }
            }));
            return ResultCompletionSource.Task;
        }

        public Task<ObservableCollection<Message>> GetMessage(string roomId) {

            TaskCompletionSource<ObservableCollection<Message>> ResultCompletionSource = new TaskCompletionSource<ObservableCollection<Message>>();
            ReatimeListener = Conn.Collection("messages").WhereEqualTo("roomId",roomId).OrderBy("sendDateTime")
                .AddSnapshotListener(new EventListener((Java.Lang.Object obj, FirebaseFirestoreException exception) =>
            {
                if (exception == null)
                {
                    QuerySnapshot querySnapshot = (QuerySnapshot)obj;                    
                    if (!querySnapshot.IsEmpty)
                    {
                        foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                        {
                            if (documentChange.GetType() == DocumentChange.Type.Added)
                            {
                                var temp = documentChange.Document.Data;
                                var tempMessage = InitialMessage(documentChange.Document.Id, temp);
                                messages.Add(tempMessage);

                                if (tempMessage.IsRead != null && !tempMessage.IsRead.ContainsKey(UserSetting.UserEmail.Replace(".", ":")))
                                    SetRead(tempMessage);

                            }
                            else if (documentChange.GetType() == DocumentChange.Type.Modified)
                            {
                                messages[documentChange.OldIndex]=InitialMessage(documentChange.Document.Id, documentChange.Document.Data);
                            }
                            else if (documentChange.GetType() == DocumentChange.Type.Removed) {
                                messages.RemoveAt(documentChange.OldIndex);
                            }
                        }

                        foreach (Message tempMessage in messages)
                        {
                            if (tempMessage.IsRead != null)
                            {
                                var tempStatus = tempMessage.IsRead.ContainsKey(UserSetting.UserEmail.Replace(".", ":"));
                                if (tempStatus&&tempMessage.IsRead[UserSetting.UserEmail.Replace(".",":")])
                                {
                                    var indexOf = messages.IndexOf(tempMessage);
                                    messages[indexOf].Text = "--This message was destructed--";
                                    messages[indexOf].AttachmentUri = "";
                                    messages[indexOf].AttachmentFileName = "";
                                }
                            }
                        }
                        ResultCompletionSource.TrySetResult(messages);
                    }
                    else
                    {
                        ResultCompletionSource.TrySetResult(null);
                    }
                }
                else {
                    Debug.Write(exception.Message);
                }
              }));
                return ResultCompletionSource.Task;
            }

        public void SetMessage(ChatRoom chatRoom,Message message)
        {
            var date = new Java.Util.Date();
            var addMessage = new Dictionary<string, Java.Lang.Object>{
                { "roomId",chatRoom.RoomID },
                { "senderEmail",message.SenderEmail },
                { "senderName",message.SenderName },
                { "text",message.Text },
                { "attachment",message.AttachmentUri},
                { "attachmentFileName",message.AttachmentFileName},
                { "sendDateTime",date},
                { "isDestruct",chatRoom.IsDestruct},
                { "UserRead",null}
            };
            Conn.Collection("messages").Add(addMessage);
        }

        public void SetUser(string email, string name)
        {  
            var NewUser = new Dictionary<string, Java.Lang.Object>
            {
                { "email",email},
                { "username", "" }
            };
            Conn.Collection(usercollection).Document(email).Set(NewUser);
        }

        public Task<string> GetUsername(string TempEmail)
        {
            TaskCompletionSource<string> ResultCompletionSource = new TaskCompletionSource<string>();
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);
            Path.Get().AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task
                 obj)=>
            {
                if (obj.IsSuccessful)
                {
                    DocumentSnapshot temp = (DocumentSnapshot)obj.Result;
                    if (temp.Exists())
                    {
                        var TempDictionary = temp.Data;
                        ResultCompletionSource.SetResult((string)TempDictionary["username"]);
                    }
                    else
                    {
                        ResultCompletionSource.SetResult(null);
                    }
                   
                }
            }));
            return ResultCompletionSource.Task;
        }

        public Task<User> SearchUser(string TempEmail) {

            TaskCompletionSource<User> ResultCompletionSource = new TaskCompletionSource<User>();
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);

            Path.Get().AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task
                 obj) =>
            {
                if (obj.IsSuccessful)
                {
                    App.searchResult = new User();
                    DocumentSnapshot temp = (DocumentSnapshot)obj.Result;
                    if (temp.Exists())
                    {
                        var TempDictionary = temp.Data;
                        var searchUserResult = new User((string)temp.Id, (string)TempDictionary["username"]);
                        ResultCompletionSource.SetResult(searchUserResult);
                    }
                    else {
                        ResultCompletionSource.SetResult(null);
                    }
                }
            }));

            return ResultCompletionSource.Task;
        }

        public void UpdateUser(string TempEmail,string TempUsername) {
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);
            Path.Update("username", TempUsername);
        }

        public Task<List<User>> GetFriendsList()
        {
            TaskCompletionSource<List<User>> ResultCompletionSource = new TaskCompletionSource<List<User>>();
            List<User> FriendsList = new List<User>();
            Conn.Collection(friendsListCollection).Document(UserSetting.UserEmail).Collection("friend").Get().AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task
                 obj) =>
            {
                if (obj.IsSuccessful == true)
                {
                    QuerySnapshot TempDocSnapshot = (QuerySnapshot)obj.Result;
                        foreach (DocumentSnapshot documentSnapshot in TempDocSnapshot.Documents)
                        {

                        var tempData = documentSnapshot.Data;
                        FriendsList.Add(new User((string)tempData["email"], (string)tempData["username"]));

                        }
                        App.FriendsList = FriendsList;

                    ResultCompletionSource.SetResult(FriendsList);
                }
            }));

            return ResultCompletionSource.Task;
        }

        public void AddFriend(User user) {
            DocumentReference Path = Conn.Collection(usercollection).Document(user.Email);
            var NewUser = new Dictionary<string, Java.Lang.Object>
                        {
                            { "email",user.Email.ToLower()},
                            { "username", user.UserName}
                        };
            Conn.Collection("friendsList").Document(UserSetting.UserEmail).Collection("friend").Document(user.Email).Set(NewUser);
        }

        //Helper Methods
        public Message InitialMessage(string docId,IDictionary<string, Java.Lang.Object> messageFrmDB)
        {

            var attachment = (string)messageFrmDB["attachment"];
            var attachmentFileName = (string)messageFrmDB["attachmentFileName"];
            var sendDateTime = (Java.Util.Date)messageFrmDB["sendDateTime"];
            var senderEmail = (string)messageFrmDB["senderEmail"];
            var senderName = (string)messageFrmDB["senderName"];
            var text = (string)messageFrmDB["text"];
            var isDestruct = (bool)messageFrmDB["isDestruct"];
            var UserRead = messageFrmDB["UserRead"];
            var IsRead = new Dictionary<string, bool>();

            //convert time
            DateTime castSendDateTime = JavaTimeToCSTime(sendDateTime);

            //convert Android.runtime.JavaDictionary to Dictionary
            if (UserRead != null)
            {

                var KeyString = new List<string>();
                var ValueBool = new List<bool>();
                var TempUserRead = (Android.Runtime.JavaDictionary)UserRead;

                foreach (string strKey in TempUserRead.Keys)
                {
                    KeyString.Add(strKey);
                    var checkBool = TempUserRead[strKey].ToString();
                    ValueBool.Add(Boolean.Parse(checkBool));
                }

                for (int a = 0; a < KeyString.Count; a++)
                {
                    IsRead.Add(KeyString[a], ValueBool[a]);
                }
            }
            Message message = new Message(docId,senderEmail, senderName, text, attachment, attachmentFileName, castSendDateTime, isDestruct, IsRead);
            return message;
        }

        public DateTime JavaTimeToCSTime(Java.Util.Date dateTime) {
            Java.Text.SimpleDateFormat SDF = new Java.Text.SimpleDateFormat("dd/MM/yyyy h:mm:ss a");
            string strSendDateTime = SDF.Format(dateTime);
            DateTime castSendDateTime = DateTime.ParseExact(strSendDateTime, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            return castSendDateTime;
        }

        public void SetRead(Message message)
        {
            var ReadMessage = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();

            if (message.IsRead.Count != 0) {
                foreach (string key in message.IsRead.Keys)
                {

                    ReadMessage.Add(key, message.IsRead[key]);
                }
            }
            
             ReadMessage.Add ( UserSetting.UserEmail.Replace(".", ":"), false);
           
            Conn.Collection("messages").Document(message.MessageId).Update("UserRead", ReadMessage);
        }

        public void ClearMessages() {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.messages = new ObservableCollection<Message>();
        }

        public void ClearAllRooms() {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.Rooms = new ObservableCollection<object>();
        }


        public Task<IndividualChatRoom> AddIndividualChatRoom(User user)
        {

            TaskCompletionSource<IndividualChatRoom> ResultCompletionSource = new TaskCompletionSource<IndividualChatRoom>();
            Android.Runtime.JavaDictionary<string, Java.Lang.Object> chatRoomUser = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>
            {
                { user.Email.Replace(".", ":"), false },
                { UserSetting.UserEmail.Replace(".", ":"), true }
            };

            var chatRoom = new Dictionary<string, Java.Lang.Object>
            {
                { "image", null },
                { "roomTitle", null },
                { "users", chatRoomUser },
                { "isGroup", false },
                { "isDestruct",false}
            };

            Conn.Collection("roomList").Add(chatRoom).AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj) => {
                if (obj.IsSuccessful)
                {
                    DocumentReference documentReference = (DocumentReference)obj.Result;
                    IndividualChatRoom temp = new IndividualChatRoom(documentReference.Id, null, user, false);
                    ResultCompletionSource.SetResult(temp);
                }
                else
                {
                    ResultCompletionSource.SetResult(null);
                }
            }));
            return ResultCompletionSource.Task;
        }
        public Task<GroupChatRoom> AddGroupChatRoom(GroupChatRoom groupChat)
        {
            TaskCompletionSource<GroupChatRoom> ResultCompletionSource = new TaskCompletionSource<GroupChatRoom>();
            var members = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();

            foreach (User user in groupChat._Users)
            {
                members.Add(user.Email.ToLower(), true);
            }

            var GroupChatRoom = new Dictionary<string, Java.Lang.Object> {
                { "image", null },
                { "roomTitle", groupChat.RoomTilte },
                {"admin",UserSetting.UserEmail.ToLower() },
                { "users", members },
                { "isGroup", true },
                { "isDestruct",false}
            };

            Conn.Collection("roomList").Add(GroupChatRoom).AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task task) => {
                if (task.IsSuccessful)
                {
                    DocumentReference documentReference = (DocumentReference)task.Result;
                    GroupChatRoom temp = new GroupChatRoom(documentReference.Id, groupChat.RoomTilte, UserSetting.UserEmail.ToLower(), groupChat._Users, false);
                    ResultCompletionSource.SetResult(temp);
                }
                else { ResultCompletionSource.SetResult(null); }

            }));

            return ResultCompletionSource.Task;
        }

        public Task<ObservableCollection<object>> GetAllRoom()
        {
            TaskCompletionSource<ObservableCollection<object>> ResultCompletionSource = new TaskCompletionSource<ObservableCollection<object>>();
            
            Query query = Conn.Collection("roomList").WhereEqualTo("users." + UserSetting.UserEmail.Replace(".",":"), true);
            ReatimeListener=query.AddSnapshotListener(new EventListener(async (Java.Lang.Object obj,FirebaseFirestoreException e) =>
            {
                if (e != null)
                {
                    ResultCompletionSource.SetException(e);
                    return;
                }
                else
                {
                    QuerySnapshot querySnapshot = (QuerySnapshot)obj;
                    if (!querySnapshot.IsEmpty) {
                        foreach (DocumentChange documentChange in querySnapshot.DocumentChanges) {
                            if (documentChange.GetType() == DocumentChange.Type.Added) {
                                var temp = documentChange.Document.Data;
                                var Id = documentChange.Document.Id;
                                var image = (string)temp["image"];
                                var isDestruct = (bool)temp["isDestruct"]; 
                                var roomTitle = (string)temp["roomTitle"];
                                var users = (Android.Runtime.JavaDictionary)temp["users"];

                                string admin;
                                var isGroup = (bool)temp["isGroup"];

                                if (isGroup)
                                {
                                    admin = (string)temp["admin"];

                                    ObservableCollection<User> GroupMembers = new ObservableCollection<User>();

                                    foreach (string key in users.Keys)
                                    {
                                        GroupMembers.Add(new User(key.Replace(":", ".")));
                                    }
                                    Rooms.Add(new GroupChatRoom(Id, roomTitle, admin, GroupMembers, isDestruct));
                                }
                                else {

                                    string RecipientEmail = null;
                                    if (users.Keys.Count ==2) {
                                        foreach (string key in users.Keys)
                                        {
                                            if (!key.Replace(":", ".").Equals(UserSetting.UserEmail)) {
                                                RecipientEmail = key.Replace(":",".");
                                            }
                                        }
                                    }
                                    string RecipientName = await GetUsername(RecipientEmail);
                                    Rooms.Add(new IndividualChatRoom(Id, RecipientName, new User(RecipientEmail,RecipientName), isDestruct));
                                }
                            }
                        }
                    }
                }

            }));
            ResultCompletionSource.SetResult(Rooms);
            return ResultCompletionSource.Task;
        }

        public Task<object> SearchChatRoomById(string roomId) {

            ChatRoom chatRoom=null;
            TaskCompletionSource<object> ResultCompletionSource = new TaskCompletionSource<object>();
            Conn.Collection("roomList").Document(roomId).Get().AddOnCompleteListener(new OnCompleteEventHandleListener(async (Android.Gms.Tasks.Task obj) => {

                if (obj.IsSuccessful)
                {
                    DocumentSnapshot documentSnapshot = (DocumentSnapshot)obj.Result;

                    var temp = documentSnapshot.Data;
                    var Id = documentSnapshot.Id;
                    var image = (string)temp["image"];
                    var isDestruct = (bool)temp["isDestruct"];
                    var roomTitle = (string)temp["roomTitle"];
                    var users = (Android.Runtime.JavaDictionary)temp["users"];

                    string admin;
                    var isGroup = (bool)temp["isGroup"];

                    if (isGroup)
                    {
                        admin = (string)temp["admin"];

                        ObservableCollection<User> GroupMembers = new ObservableCollection<User>();

                        foreach (string key in users.Keys)
                        {
                            GroupMembers.Add(new User(key.Replace(":", ".")));
                        }
                        chatRoom = new GroupChatRoom(Id, roomTitle, admin, GroupMembers, isDestruct);
                    }
                    else
                    {

                        string RecipientEmail = null;
                        if (users.Keys.Count == 2)
                        {
                            foreach (string key in users.Keys)
                            {
                                if (!key.Replace(":", ".").Equals(UserSetting.UserEmail))
                                {
                                    RecipientEmail = key.Replace(":", ".");
                                }

                            }

                            string RecipientName = await GetUsername(RecipientEmail);
                            chatRoom = new IndividualChatRoom(Id, RecipientName, new User(RecipientEmail, RecipientName), isDestruct);
                        }

                    }
                }
            }));
            ResultCompletionSource.SetResult(chatRoom);
            return ResultCompletionSource.Task;
        }
    }
}