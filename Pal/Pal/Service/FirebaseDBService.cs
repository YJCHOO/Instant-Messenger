using Pal.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Pal.Service
{
    public interface IFirebaseDatabase
    {
        Task<string> GetUsername(string email);
        Task<bool> SetUser(string email, string name);
        Task<bool> UpdateUser(string email, string username);
        Task<bool> SetMessage(ChatRoom chatRoom,Message message);
        Task<ObservableCollection<Message>> GetMessage(string roomId);
        Task<bool> SetRead(Message message);    
        Task<IndividualChatRoom> AddIndividualChatRoom(User user);
        Task<GroupChatRoom> AddGroupChatRoom(GroupChatRoom groupChat);
        Task<IndividualChatRoom> SearchIndividualChatRoom(string userEmail);
        Task<ObservableCollection<object>> GetAllRoom();
        Task<ObservableCollection<Board>> GetAllPinBoardMessage(string roomID);
        Task<bool> AddPinBoardMessage(string roomID, Board boardMessage);
        Task<string> SetRoomDestruct(string roomId, bool destructStatus);
        Task<bool> DestructMessage(string MessagesId);
        void AddFriend(User user);
        Task<List<User>> GetFriendsList();
        Task<User> SearchUser(string email);
        Task<object> SearchChatRoomById(string roomId);
        Task<bool> RemoveFriend(string FriendEmail);
        Task<bool> RemovePinBoardMessage(string PinBoardMessageId);
        Task<bool> AddPoll(Poll poll);
        Task<Poll> GetLastestPoll(string roomId);
        Task<bool> UpdateResult(Poll poll);
        Task<bool> UpdatePollCloseStatus(string PollId);
        Task<bool> CreateMoment(Moment moment);
        Task<ObservableCollection<Moment>> GetMomentsList();
        void ClearAllMoments();
        void ClearMessages();
        void ClearAllRooms();
        void ClearAllPinBoardMessage();
    }
}
