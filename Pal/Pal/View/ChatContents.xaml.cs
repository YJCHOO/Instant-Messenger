using Pal.Model;
using Pal.ViewModel;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatContents : ContentPage
	{
        ChatContentsViewModel _chatRoom;
        bool IsLeave = true;

        public ChatContents(IndividualChatRoom SelectedUser) {
            InitializeComponent();
            this.BindingContext = _chatRoom = new ChatContentsViewModel(SelectedUser);
            this.Title = SelectedUser._User.UserName;
        }

        public ChatContents(GroupChatRoom groupChat) {
            InitializeComponent();
            this.BindingContext = _chatRoom = new ChatContentsViewModel(groupChat);
            this.Title = groupChat.RoomTilte;
        }

        protected override async void OnDisappearing() {
            if (IsTappedToViewAttachment()) {
                return;
            }

            if (IsLeave)
            {
                await _chatRoom.OnDisappearing();
            }
        }

        protected override async void OnAppearing() {
            if (IsTappedToViewAttachment()) {
                return;
            }

            if (IsLeave)
            {
                await _chatRoom.OnAppearing();
            }
            IsLeave = true;
        }

        public bool IsTappedToViewAttachment() {

            var currentPage = App.Current.MainPage.Navigation.NavigationStack;
            if (currentPage[currentPage.Count - 1].GetType().ToString().Equals("Pal.View.WebViewAttachment"))
            {
                return true;
            }
            else { return false; }
            
        }

        private async void DestructTimer_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("After Read Self-Destruct ? ", "Cancel", null,"Yes","No");

            bool Self_destruct = false;

            if (action != null)
            {
                if (action.CompareTo("Yes") == 0)
                {

                    Self_destruct = true;
                }
            }
            _chatRoom.ChatRoom.IsDestruct = Self_destruct;
            await _chatRoom.UpdateRoomDestructStatus(Self_destruct);
        }

        private async void AddAttachments_Clicked(object sender, EventArgs e)
        {
            IsLeave = false;
            var isPicked = await _chatRoom.PickAndShowFile();
            if (isPicked)
            {
                _chatRoom.DisplayAttachment();
            }
        }

        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            _chatRoom.RemoveAttachment();
        }
    }
}