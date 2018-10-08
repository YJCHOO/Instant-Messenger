using Pal.Model;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatsPage : ContentPage
	{
        public ObservableCollection<ChatRoom> ChatRooms { get; set; }

        public ChatsPage ()
		{
            ChatRooms = new ObservableCollection<ChatRoom>();
            User TempUser = new User("John", "blank-profile-picture-640.png");
            ChatRoom TempChatRoom = new ChatRoom("Testing",new ObservableCollection<User> { TempUser});
            ChatRooms.Add(TempChatRoom);

            InitializeComponent ();

            ChatRoomList.ItemsSource = ChatRooms;
        }


        private void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            this.Navigation.PushAsync(new ChatContents(e.Item));
        }

    }
}