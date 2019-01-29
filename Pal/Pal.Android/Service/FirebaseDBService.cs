using Xamarin.Forms;
using Pal.Service;
using Firebase.Firestore;
using System.Collections.Generic;
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
        private const string pinBoardCollection = "pinBoard";
        private const string pollCollection = "poll";
        private const string momentCollection = "moment";

        private FirebaseFirestore Conn = FirebaseFirestore.GetInstance(MainActivity.app);
        private IListenerRegistration ReatimeListener = null;
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private ObservableCollection<object> Rooms = new ObservableCollection<object>();
        private ObservableCollection<Board> board = new ObservableCollection<Board>();
        private ObservableCollection<Moment> Moments = new ObservableCollection<Moment>();

        //Create
        public Task<IndividualChatRoom> AddIndividualChatRoom(User user)
        {
            TaskCompletionSource<IndividualChatRoom> ResultCompletionSource = new TaskCompletionSource<IndividualChatRoom>();
            Android.Runtime.JavaDictionary<string, Java.Lang.Object> chatRoomUser = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>
            {
                { user.Email.Replace(".", ":"), true },
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
            var members = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>{
                { UserSetting.UserEmail.ToLower().Replace(".", ":"), true }};

            foreach (User user in groupChat._Users){
                members.Add(user.Email.ToLower().Replace(".", ":"), true);}

            var GroupChatRoom = new Dictionary<string, Java.Lang.Object> {
                { "image", null },
                { "roomTitle", groupChat.RoomTilte },
                {"admin",UserSetting.UserEmail.ToLower() },
                { "users", members },
                { "isGroup", true },
                { "isDestruct",false}};

            Conn.Collection("roomList").Add(GroupChatRoom)
                .AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task task) => {
                if (task.IsSuccessful)
                {
                    DocumentReference documentReference = (DocumentReference)task.Result;
                    GroupChatRoom temp = 
                        new GroupChatRoom(documentReference.Id, groupChat.RoomTilte,
                        UserSetting.UserEmail.ToLower(), groupChat._Users, false);
                    ResultCompletionSource.SetResult(temp);
                }
                else { ResultCompletionSource.SetResult(null); }
            }));
            return ResultCompletionSource.Task;
        }
        public void AddFriend(User user)
        {
            DocumentReference Path = Conn.Collection(usercollection).Document(user.Email);
            var NewUser = new Dictionary<string, Java.Lang.Object>
                        {
                            { "email",user.Email.ToLower()},
                            { "username", user.UserName}
                        };
            Conn.Collection("friendsList").Document(UserSetting.UserEmail).Collection("friend").Document(user.Email).Set(NewUser);
        }
        public Task<bool> AddPinBoardMessage(string roomID, Board boardMessage)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var PinBoardMessage = new Dictionary<string, Java.Lang.Object>
            {
                { "roomId", roomID },
                {"title",boardMessage.Title},
                {"description",boardMessage.Description },
                {"attachmentFileName",boardMessage._Attachment.FileName},
                {"attachment",boardMessage._Attachment.AttachmentUri },
                {"userName",boardMessage._User.UserName},
                {"userEmail",boardMessage._User.Email },
                { "dateTime",new Java.Util.Date()}
            };

            Conn.Collection(pinBoardCollection).Add(PinBoardMessage)
                .AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj) => {
                    if (obj.IsSuccessful)
                        ResultCompletionSource.SetResult(true);
                    else
                        ResultCompletionSource.SetResult(false);
                }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> AddPoll(Poll poll) {

            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();

            var Option = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();
            var Result = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();
            foreach ( string _Option in poll._Option ) {
                Option.Add(_Option, true);
            }

            var Poll = new Dictionary<string, Java.Lang.Object>
            {
                {"roomID",poll.RoomId },
                {"title",poll.Title },
                {"isClose",poll.IsClose },
                {"option",Option },
                {"result",Result},
                { "dateTime",new Java.Util.Date()}
            };

            Conn.Collection(pollCollection).Add(Poll).AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=>{

                if (obj.IsSuccessful)
                {
                    ResultCompletionSource.SetResult(true);
                }
                else
                {
                    ResultCompletionSource.SetResult(false);
                }
            }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> CreateMoment(Moment moment) {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();

            var receiver = new Android.Runtime.JavaDictionary<string, bool>();
            foreach (string key in moment.Receiver.Keys) {
                receiver.Add(key, false);
            }

            var _Moment = new Dictionary<string, Java.Lang.Object>
            {
                {"sender",moment.Sender},
                {"senderName", moment.SenderName},
                {"receivers",receiver },
                {"dateTime",new Java.Util.Date() },
                {"description",moment.Description },
                {"attachment",moment._Attachment.AttachmentUri }

            };

            var Path = Conn.Collection(momentCollection).Add(_Moment);
            Path.AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=> 
            {
                if (obj.IsSuccessful)
                {
                    ResultCompletionSource.SetResult(true);
                }
                else
                {
                    ResultCompletionSource.SetResult(false);
                }
            }));
            return ResultCompletionSource.Task;
        }

        //Upadate
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
        public Task<bool> SetMessage(ChatRoom chatRoom, Message message)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
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
            Conn.Collection("messages").Add(addMessage).
                AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=> 
                {
                    if (obj.IsSuccessful)
                        ResultCompletionSource.SetResult(true);
                    else
                        ResultCompletionSource.SetResult(false);
                }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> SetUser(string email, string name)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var NewUser = new Dictionary<string, Java.Lang.Object>
            {
                { "email",email},
                { "username", "" }
            };
            Conn.Collection(usercollection).Document(email).Set(NewUser)
                .AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=> 
            {
                if (obj.IsSuccessful)
                    ResultCompletionSource.SetResult(true);
                else
                    ResultCompletionSource.SetResult(false);
            }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> UpdateUser(string TempEmail, string TempUsername)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            DocumentReference Path = Conn.Collection(usercollection).Document(TempEmail);
            Path.Update("username", TempUsername)
                .AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=>
                {
                    if (obj.IsSuccessful) {
                        ResultCompletionSource.SetResult(true);
                    }
                    else
                    {
                        ResultCompletionSource.SetResult(false);
                    }
                }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> SetRead(Message message)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var ReadMessage = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();

            if (message.IsRead.Count != 0)
            {
                foreach (string key in message.IsRead.Keys)
                {

                    ReadMessage.Add(key, message.IsRead[key]);
                }
            }
            ReadMessage.Add(UserSetting.UserEmail.Replace(".", ":"), false);

            Conn.Collection("messages").Document(message.MessageId).Update("UserRead", ReadMessage)
                .AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=> {
                    if (obj.IsSuccessful)
                        ResultCompletionSource.SetResult(true);
                    else
                        ResultCompletionSource.SetResult(false);
                }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> UpdateResult(Poll poll)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            Android.Runtime.JavaDictionary<string, Java.Lang.Object> Result = new Android.Runtime.JavaDictionary<string, Java.Lang.Object>();
            foreach (string email in poll.Result.Keys)
            {
                Result.Add(email, poll.Result[email].ToString());
            }

            var Path = Conn.Collection(pollCollection).Document(poll.PollId);
            Path.Update("result", Result).AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj) => 
            {
                if (obj.IsSuccessful)
                {
                    ResultCompletionSource.SetResult(true);
                }
                else
                {
                    ResultCompletionSource.SetResult(false);
                }
            }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> UpdatePollCloseStatus(string PollId) {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var Path = Conn.Collection(pollCollection).Document(PollId);
            Path.Update("isClose", true).AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj) => {
                if (obj.IsSuccessful)
                {

                    ResultCompletionSource.SetResult(true);
                }
                else {
                    ResultCompletionSource.SetResult(false);
                }
                
            }));
            return ResultCompletionSource.Task;
        }


        //Retrieve
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
            TaskCompletionSource<ObservableCollection<Message>> ResultCompletionSource = 
                new TaskCompletionSource<ObservableCollection<Message>>();

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

                                if (tempMessage.IsRead != null 
                                && 
                                !tempMessage.IsRead.ContainsKey(UserSetting.UserEmail.Replace(".", ":")))
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
                    else{ ResultCompletionSource.TrySetResult(null);}
                }}));
                return ResultCompletionSource.Task;
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
        public Task<ObservableCollection<Board>> GetAllPinBoardMessage(string roomID)
        {

            TaskCompletionSource<ObservableCollection<Board>> ResultCompletionSource = new TaskCompletionSource<ObservableCollection<Board>>();
            Query query = Conn.Collection(pinBoardCollection).WhereEqualTo("roomId", roomID);
            ReatimeListener = query.AddSnapshotListener(new EventListener((Java.Lang.Object obj, FirebaseFirestoreException e) => {
                if (e != null)
                {
                    ResultCompletionSource.SetException(e);
                }
                else
                {
                    QuerySnapshot querySnapshot = (QuerySnapshot)obj;
                    if (!querySnapshot.IsEmpty)
                    {
                        foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                        {
                            if (documentChange.GetType() == DocumentChange.Type.Added)
                            {
                                var temp = documentChange.Document.Data;
                                var BoardMessageId = documentChange.Document.Id;
                                var Attachment = (string)temp["attachment"];
                                var AttachmentFileName = (string)temp["attachmentFileName"];
                                var Description = (string)temp["description"];
                                var RoomId = (string)temp["roomId"];
                                var Title = (string)temp["title"];
                                var UserEmail = (string)temp["userEmail"];
                                var UserName = (string)temp["userName"];
                                var dateTime = (Java.Util.Date)temp["dateTime"];
                                Attachment attachment = new Attachment(AttachmentFileName, Attachment);
                                User user = new User(UserEmail, UserName);


                                if (AttachmentFileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || AttachmentFileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    attachment.Thumbnail = attachment.AttachmentUri;
                                }
                                else if (AttachmentFileName.EndsWith(".mp4", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    attachment.Thumbnail = "round_movie_black_48.png";
                                }
                                else
                                {
                                    attachment.Thumbnail = "round_insert_drive_file_black_48.png";

                                }

                                board.Add(new Board(BoardMessageId, Title, Description, attachment, user, JavaTimeToCSTime(dateTime)));

                            }

                        }
                        ResultCompletionSource.TrySetResult(board);
                    }
                    else
                    {
                        ResultCompletionSource.TrySetResult(null);
                    }

                }


            }));
            return ResultCompletionSource.Task;
        }
        public Task<ObservableCollection<object>> GetAllRoom()
        {
            TaskCompletionSource<ObservableCollection<object>> ResultCompletionSource = new TaskCompletionSource<ObservableCollection<object>>();

            Query query = Conn.Collection("roomList").WhereEqualTo("users." + UserSetting.UserEmail.Replace(".", ":"), true);
            ReatimeListener = query.AddSnapshotListener(new EventListener(async (Java.Lang.Object obj, FirebaseFirestoreException e) =>
            {
                if (e != null)
                {
                    Debug.Write("Error In Gett All Room : " + e.Message);
                    
                    return;
                }
                else
                {
                    QuerySnapshot querySnapshot = (QuerySnapshot)obj;
                    if (!querySnapshot.IsEmpty)
                    {
                        foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                        {
                            if (documentChange.GetType() == DocumentChange.Type.Added)
                            {
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
                                    }
                                    string RecipientName = await GetUsername(RecipientEmail);
                                    Rooms.Add(new IndividualChatRoom(Id, RecipientName, new User(RecipientEmail, RecipientName), isDestruct));
                                }
                            }
                        }
                        ResultCompletionSource.TrySetResult(Rooms);
                    }
                    else {
                        ResultCompletionSource.SetResult(null);
                    }
                }
            }));
            
            return ResultCompletionSource.Task;
        }
        public Task<object> SearchChatRoomById(string roomId)
        {
            ChatRoom chatRoom = null;
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
        public Task<Poll> GetLastestPoll(string roomId) {

            TaskCompletionSource<Poll> ResultCompletionSource = new TaskCompletionSource<Poll>();
            var Path = Conn.Collection(pollCollection);
            Path.WhereEqualTo("roomID",roomId).OrderBy("dateTime", Query.Direction.Descending).Limit(1).AddSnapshotListener(new EventListener((Java.Lang.Object obj, FirebaseFirestoreException e) =>
             {
                 if (e != null)
                 {
                     Debug.Write(e.Message);
                     return;
                 }
                 else
                 {
                     QuerySnapshot querySnapshot = (QuerySnapshot)obj;
                     if (!querySnapshot.IsEmpty)
                     {
                         foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                         {
                             if (documentChange.GetType() == DocumentChange.Type.Added)
                             {
                                 var temp = documentChange.Document.Data;
                                 var Id = (string)documentChange.Document.Id;
                                 var RoomId = (string)temp["roomID"];
                                 var Title = (string)temp["title"];
                                 var IsClose = (bool)temp["isClose"];
                                 var Option = (Android.Runtime.JavaDictionary)temp["option"];
                                 var Result = (Android.Runtime.JavaDictionary)temp["result"];

                                 ObservableCollection<string> _Option = new ObservableCollection<string>();
                                 var _Result = new Dictionary<string, string>();
                                 if (Option.Count != 0)
                                 {
                                     foreach (string key in Option.Keys)
                                     {
                                         _Option.Add(key);
                                     }
                                 }
                                 if (Result!=null)
                                 {
                                     
                                     foreach (string key in Result.Keys)
                                     {
                                         _Result.Add(key, Result[key].ToString());
                                     }
                                 }
                                 Poll poll = new Poll(Id, RoomId, Title, IsClose, _Option, _Result);
                                 ResultCompletionSource.TrySetResult(poll);
                             }
                             else
                             {

                                 ResultCompletionSource.TrySetResult(null);
                             }
                         }
                     }
                 }
             }));

            return ResultCompletionSource.Task;
        }
        public Task<ObservableCollection<Moment>> GetMomentsList()
        {
            TaskCompletionSource<ObservableCollection<Moment>> ResultCompletionSource = new TaskCompletionSource<ObservableCollection<Moment>>();

            Java.Util.Calendar cal = Java.Util.Calendar.Instance;
            Java.Util.Date CurrentDateTime = cal.Time;
            cal.Add(Java.Util.Calendar.Date, -1);
            Java.Util.Date YesterdayDateTime = cal.Time;

            Debug.Write(CurrentDateTime);
            Debug.Write(YesterdayDateTime);

            var Path = Conn.Collection(momentCollection)
                .WhereEqualTo("receivers." + UserSetting.UserEmail.Replace(".", ":"), false)
                .WhereGreaterThanOrEqualTo("dateTime", YesterdayDateTime)
                .WhereLessThanOrEqualTo("dateTime", CurrentDateTime)
                .OrderBy("dateTime");

            ReatimeListener = Path.AddSnapshotListener(new EventListener((Java.Lang.Object obj, FirebaseFirestoreException e) => {
                if (e != null) {
                    Debug.Write(e.Message);
                    ResultCompletionSource.TrySetResult(null);
                    return;
                }
                else {
                    QuerySnapshot querySnapshot = (QuerySnapshot)obj;
                    if (!querySnapshot.IsEmpty)
                    {
                        foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                        {
                            if (documentChange.GetType() == DocumentChange.Type.Added)
                            {
                                var temp = documentChange.Document.Data;
                                var Id = documentChange.Document.Id;
                                var Attachment = (string)temp["attachment"];
                                var DateTime = (Java.Util.Date)temp["dateTime"];
                                var Description = (string)temp["description"];
                                var Sender = (string)temp["sender"];
                                var SenderName = (string)temp["senderName"];
                                string thumbnail = null;
                                if (Attachment != null)
                                {
                                    if (Attachment.Contains("jpg", StringComparison.OrdinalIgnoreCase) || Attachment.Contains("png", StringComparison.OrdinalIgnoreCase))
                                    {
                                        thumbnail = Attachment;
                                    }
                                    else
                                    {
                                        thumbnail = "round_movie_black_48.png";
                                    }
                                }
                                Attachment attachment = new Attachment();
                                attachment.AttachmentUri = Attachment;
                                attachment.Thumbnail = thumbnail;
                                    Moments.Add(new Moment(Id, Sender, SenderName, JavaTimeToCSTime(DateTime), Description, attachment));
                            }
                        }
                    }

                    ResultCompletionSource.TrySetResult(Moments);
                }
                }));
            return ResultCompletionSource.Task;
        }

        //Remove
        public Task<bool> RemovePinBoardMessage(string PinBoardMessageId) {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var Path =  Conn.Collection(pinBoardCollection).Document(PinBoardMessageId);
            Path.Delete().AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj)=>
            {
                if (obj.IsSuccessful)
                {
                    ResultCompletionSource.SetResult(true);
                }
                else
                {
                    ResultCompletionSource.SetResult(false);
                }
            }));
            return ResultCompletionSource.Task;
        }
        public Task<bool> RemoveFriend(string FriendEmail)
        {
            TaskCompletionSource<bool> ResultCompletionSource = new TaskCompletionSource<bool>();
            var Path = Conn.Collection(friendsListCollection).Document(UserSetting.UserEmail).Collection("friend").Document(FriendEmail);
            Path.Delete().AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task obj) =>
            {
                if (obj.IsSuccessful)
                {
                    ResultCompletionSource.SetResult(true);
                }
                else
                {
                    ResultCompletionSource.SetResult(false);
                }
            }));
            return ResultCompletionSource.Task;
        }

        //Helper Methods
        public Message InitialMessage(string docId, IDictionary<string, Java.Lang.Object> messageFrmDB)
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
            Message message = new Message(docId, senderEmail, senderName, text, attachment, attachmentFileName, castSendDateTime, isDestruct, IsRead);
            return message;
        }
        public DateTime JavaTimeToCSTime(Java.Util.Date dateTime)
        {
            Java.Text.SimpleDateFormat SDF = new Java.Text.SimpleDateFormat("dd/MM/yyyy h:mm:ss a");
            string strSendDateTime = SDF.Format(dateTime);
            DateTime castSendDateTime = DateTime.ParseExact(strSendDateTime, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            return castSendDateTime;
        }
        public void ClearMessages()
        {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.messages = new ObservableCollection<Message>();
        }
        public void ClearAllRooms()
        {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.Rooms = new ObservableCollection<object>();
        }
        public void ClearAllPinBoardMessage()
        {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.board = new ObservableCollection<Board>();
        }
        public void ClearAllMoments() {
            if (ReatimeListener != null)
            {
                ReatimeListener.Remove();
                ReatimeListener = null;
            }
            this.Moments = new ObservableCollection<Moment>();
        }
    }
}