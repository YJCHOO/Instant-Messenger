using Pal.Model;
using Pal.Service;
using Pal.View;
using Pal.View.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    public class ChatsPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<object> ChatRooms { get; set; }
        public ICommand OnAddChatCommand { get; set; }
        public ICommand OnFriendsListCommand { get; set; }
        public ICommand OnLogoutCommand { get; set; }
        public bool ListIsEmpty { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatsPageViewModel() {

            //FriendsList
            OnFriendsListCommand = new Command(async() => {
                await App.Current.MainPage.Navigation.PushAsync(new FriendListPage());
            });
            //Logout
            OnLogoutCommand = new Command(() =>
            {
                UserSetting.ClearEverything(); //clear all User data.
                App.Current.MainPage= new NavigationPage(new AuthenticationPage()); //go back logout screen
            });
        }

        protected virtual void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        public async Task InitialRoom() {
            ChatRooms = await DependencyService.Get<IFirebaseDatabase>().GetAllRoom();
            if (ChatRooms == null)
            {
                ListIsEmpty = true;
            }
            else
            {
                ListIsEmpty = false;
            }
            OnPropertyChanged("ListIsEmpty");
            OnPropertyChanged("ChatRooms");
        }

        public void StopListener() {
            DependencyService.Get<IFirebaseDatabase>().ClearAllRooms();
        }
    }
}
