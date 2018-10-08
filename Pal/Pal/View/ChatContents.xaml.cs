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
        public ChatContentsViewModel chatContentsViewModel;

        public ChatContents() {
            InitializeComponent();
        }

		public ChatContents (object SelectedChatRoom)
		{
			InitializeComponent ();
            ChatRoom ChatRoomDetails = new ChatRoom();
            ChatRoomDetails = (ChatRoom)SelectedChatRoom;

            this.Title = ChatRoomDetails.RoomTilte;
            this.BindingContext =  new ChatContentsViewModel();
        }

        private void OnSendBtn(object sender, EventArgs e)
        {
            chatTextInput.Text = "";
        }
    }
}