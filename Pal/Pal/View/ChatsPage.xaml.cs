using Pal.Model;
using Pal.Service;
using Pal.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatsPage : ContentPage
	{
        ChatsPageViewModel VM = new ChatsPageViewModel();

        public ChatsPage ()
		{
            InitializeComponent ();
            this.BindingContext = VM;
            base.OnAppearing();
        }

        protected override async void OnAppearing()
        {
            await VM.InitialRoom();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            VM.StopListener();

            base.OnDisappearing();
        }

        public async void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ChatRoomList.BeginRefresh();
            if (e.Item.GetType() == typeof(IndividualChatRoom))
            {
                await App.Current.MainPage.Navigation.PushAsync(new ChatContents((IndividualChatRoom)e.Item));
            }
            else {
                await App.Current.MainPage.Navigation.PushAsync(new ChatContents((GroupChatRoom)e.Item));
            }
            ChatRoomList.EndRefresh();
        }
    }
}