using Pal.Model;
using Pal.ViewModel;
using System.Collections.ObjectModel;
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
            await VM.OnAppearing();
        }


        public void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}