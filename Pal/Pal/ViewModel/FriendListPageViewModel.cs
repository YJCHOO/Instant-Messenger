using Pal.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Pal.Service;
using System.Collections.Generic;
using Pal.View;
using System.Threading.Tasks;
using System.Linq;

namespace Pal.ViewModel
{
     public class FriendListPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<User> FriendsList { get; set; }
        public ObservableCollection<User> SearchUserResult { get; set; }
        public ICommand OnDeleteFriendCommand { get; set; }
        public string SearchResultLbl { get; set; }
        public string SearchText { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public FriendListPageViewModel() {
            FriendsList = new ObservableCollection<User>();
            SearchUserResult = new ObservableCollection<User>();

            OnDeleteFriendCommand = new Command<string>(async (string FriendEmail) => 
            {

                var IsCompleted =  await DependencyService.Get<IFirebaseDatabase>().RemoveFriend(FriendEmail);
                if (IsCompleted)
                {
                    await App.Current.MainPage.DisplayAlert("Done", "User " + FriendEmail + "deleted from your FriendList", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong ....", "Friend unable to delete, please try again", "Ok");
                }

            });
        }

        public async Task<ObservableCollection<User>> GetSearchResult(string email) {

            var user = await DependencyService.Get<IFirebaseDatabase>().SearchUser(email);
            SearchUserResult.Clear();
            SearchUserResult.Add(user);
            return SearchUserResult;
        }

        public async Task<ObservableCollection<User>> OnAppearing() {
            FriendsList = await InitialFriendsListAsync();
            return FriendsList;
        }

        public async Task AddFriendToFriendsListAsync(User user)
        {
            DependencyService.Get<IFirebaseDatabase>().AddFriend(user);
            await App.Current.MainPage.DisplayAlert("Successful Message", "New friend added", "Ok");
            FriendsList.Add(user);
        }

        public async Task<IndividualChatRoom> CreateIndividualChatRoom(User user) {

            var individualChatRoom =  await DependencyService.Get<IFirebaseDatabase>().AddIndividualChatRoom(user);
            return individualChatRoom;
                
            
        }

        public async Task SearchIndividualChatRoom(User user) {
            var result = await DependencyService.Get<IFirebaseDatabase>().SearchIndividualChatRoom(user.Email);
            if (result == null)
            {
               var individualChatRoom =  await CreateIndividualChatRoom(user);
                result = individualChatRoom;
            }

            await App.Current.MainPage.Navigation.PushAsync(new ChatContents(result));

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        private async Task<ObservableCollection<User>> InitialFriendsListAsync()
        {
            List<User> Temp = await DependencyService.Get<IFirebaseDatabase>().GetFriendsList();

            ObservableCollection<User> TempFriendsList = new ObservableCollection<User>();

            foreach (User u in Temp)
            {
                TempFriendsList.Add(u);
            }
            return TempFriendsList;
        }

       
    }
}
