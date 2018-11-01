using Pal.Model;
using Pal.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatContents : ContentPage
	{
        ChatContentsViewModel _chatRoom;

        public ChatContents() {
            InitializeComponent();
        }

        public ChatContents(IndividualChatRoom SelectedUser) {
            InitializeComponent();
            this.BindingContext = _chatRoom = new ChatContentsViewModel(SelectedUser);
            this.Title = SelectedUser.RoomTilte;
        }

        protected override async void OnDisappearing() {

            await _chatRoom.OnDisappearing();
        }

        protected override async void OnAppearing() {
            await _chatRoom.OnAppearing();
        }

		////public ChatContents (object SelectedChatRoom)
		////{
		//	InitializeComponent ();
  //          ChatRoom ChatRoomDetails = new ChatRoom();
  //          ChatRoomDetails = (ChatRoom)SelectedChatRoom;

  //          this.Title = ChatRoomDetails.RoomTilte;
  //          this.BindingContext = chatContentsViewModel;
  //      }

        private void OnSendBtn(object sender, EventArgs e)
        {
            chatTextInput.Text = "";
        }

        private async void DestructTimer_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Self-Destruct Timer", "Cancel", null,"None","After Read");

            bool Self_destruct = false;

            if (action.CompareTo("After Read") ==0) {

                Self_destruct = true;
            }
            _chatRoom.ChatRoom.IsDestruct = Self_destruct;
            await _chatRoom.UpdateRoomDestructStatus(Self_destruct);
        }
    }
}