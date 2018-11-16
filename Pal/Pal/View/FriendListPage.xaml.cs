using Pal.Model;
using Pal.Service;
using Pal.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendListPage : ContentPage
    {
        private bool ClickedBtn = false;
        FriendListPageViewModel VM = new FriendListPageViewModel();

        public FriendListPage()
        {
            InitializeComponent();
            this.BindingContext = VM;
        }

        private void ListVisble() {
            ResultLbl.IsVisible = false;
            SearchUserBtn.IsVisible = false;
            FriendsListView.IsVisible = true;
        }

        private void ListInvisible() {

            ResultLbl.IsVisible = true;
            SearchUserBtn.IsVisible = true;
            FriendsListView.IsVisible = false ;
        }

        private void ResultLblVisibleOnly() {
            ResultLbl.IsVisible = true;
            SearchUserBtn.IsVisible = false;
            FriendsListView.IsVisible = false;

        }

        private void RefreshList() {

            FriendsListView.Header = "Friends";
            FriendsListView.Footer = "";
            FriendsListView.ItemsSource = null;
            FriendsListView.ItemsSource = VM.FriendsList;
        }

        protected override async void OnAppearing()
        {
            UserSearchBar.IsEnabled = false;
            FriendsListView.BeginRefresh();
           var Temp =  await VM.OnAppearing() ;
            if (Temp.Count != 0)
            {
                ListVisble();
                FriendsListView.ItemsSource = Temp;
            }
            else {
                ResultLbl.Text = "Friend list is empty.";
                ResultLblVisibleOnly();
            }
            UserSearchBar.IsEnabled = true;
            FriendsListView.EndRefresh();
        }

        private void UserSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string InputEmail = e.NewTextValue;

            FriendsListView.BeginRefresh();
            if (InputEmail != UserSetting.UserEmail)
            {
                if (!string.IsNullOrWhiteSpace(InputEmail))
                {
                    if (CountData(e.NewTextValue) != 0)
                    {
                        ListVisble();
                        FriendsListView.ItemsSource = VM.FriendsList.Where(i => i.Email.ToLower().Contains(InputEmail.ToLower()));
                    }
                    else
                    {
                        ListInvisible();
                        ResultLbl.Text = "User not in your friends list.";
                        SearchUserBtn.Text = "Search New User \"" + InputEmail + "\"";
                    }
                }
                else
                {
                    if (VM.FriendsList.Count != 0)
                    {
                        ListVisble();
                        FriendsListView.ItemsSource = VM.FriendsList;
                    }
                    else {
                        ResultLbl.Text = "Friend list is empty.";
                        ResultLblVisibleOnly();
                    }
                }
            }
            else {
                ResultLblVisibleOnly();
                ResultLbl.Text = "Hi,"+UserSetting.UserName;

            }
            FriendsListView.EndRefresh();
        }

        private int CountData(string email) {

            return VM.FriendsList.Count(i => i.Email.ToLower().Contains(email.ToLower()));
        }

        private async void SearchUserBtn_Clicked(object sender, EventArgs e)
        {
            var TempSearchUserResult = await VM.GetSearchResult(UserSearchBar.Text);
            if (TempSearchUserResult.Count != 0)
            {
                ListVisble();
                FriendsListView.Header = "Search User Result";
                FriendsListView.Footer = "Tap to Add User";
                FriendsListView.ItemsSource = TempSearchUserResult;
                ClickedBtn = true;
            }
        }

        private async void FriendsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (ClickedBtn == true)
            {
                FriendsListView.BeginRefresh();
                await VM.AddFriendToFriendsListAsync((User)e.Item);
                FriendsListView.EndRefresh();
                RefreshList();
                ClickedBtn = false;
            }
            else
            {
                FriendsListView.BeginRefresh();
                await VM.SearchIndividualChatRoom((User)e.Item);
                FriendsListView.EndRefresh();
            }
        }

        private async void NewGroup_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new InviteMembersPage(VM.FriendsList));
        }
    }
}