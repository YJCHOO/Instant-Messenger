using Pal.Model;
using Pal.Service;
using Pal.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class CreateChatRoomPageModelView : INotifyPropertyChanged
    {
        public string Search { get; set; }
        public ObservableCollection<SelectableData<User>> InviteList { get; set; }
        public ObservableCollection<User> InvitedFriends { get; set; }
        public ICommand OnNextRoomTitle { get; set; }
        public ICommand OnCreateNewGroup { get; set; }
        public string RoomTitle { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public CreateChatRoomPageModelView() {
            InviteList = new ObservableCollection<SelectableData<User>>();
            OnNextRoomTitle = new Command(async () =>
            {
                InvitedFriends = new ObservableCollection<User>();
                foreach (SelectableData<User> user in InviteList) {
                    if (user.Selected && !InvitedFriends.Contains(user.Data)) {
                        InvitedFriends.Add(user.Data);
                    }
                }

                if (InvitedFriends.Count <2) {
                    await App.Current.MainPage.DisplayAlert("Something happen....", "At least 3 members(included you) in group.", "Ok");
                    return;
                }

                await App.Current.MainPage.Navigation.PushAsync(new NewGroup(InvitedFriends));
            });
            //OnCreateNewGroup = new Command(async () => {

            //    var NewGroup = new GroupChatRoom("", RoomTitle, "",InvitedFriends, false);
            //    GroupChatRoom groupChatRoom = await DependencyService.Get<IFirebaseDatabase>().AddGroupChatRoom(NewGroup);
            //    if (groupChatRoom != null)
            //    {
            //        await App.Current.MainPage.Navigation.PopToRootAsync();
            //        await App.Current.MainPage.Navigation.PushAsync(new GroupChatContents(groupChatRoom));
            //    }
            //    else { await App.Current.MainPage.DisplayAlert("Something happen....", "Room unable to create.", "Ok"); }
            //});
        }

        public void InitialNewGroup() {
            OnPropertyChanged("InvitedFriends");
        }

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        public async Task CreateNewGroup() {
            if (string.IsNullOrWhiteSpace(RoomTitle))
            {
                await App.Current.MainPage.DisplayAlert("Something happen....", "Must have a room title.", "Ok");
            }
            else {

                GroupChatRoom groupChatRoom = new GroupChatRoom("", RoomTitle, UserSetting.UserEmail, InvitedFriends, false);
                var NewGroupChat= await DependencyService.Get<IFirebaseDatabase>().AddGroupChatRoom(groupChatRoom);
                await App.Current.MainPage.Navigation.PopToRootAsync();
                await App.Current.MainPage.Navigation.PushAsync(new GroupChatContents(NewGroupChat));
            }
        }

    }
}
