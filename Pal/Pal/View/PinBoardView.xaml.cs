using Pal.Model;
using Pal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PinBoardView : ContentPage
	{
        PinBoardViewModel VM;

        public PinBoardView (GroupChatRoom groupChatRoom)
		{
			InitializeComponent ();
           BindingContext= VM = new PinBoardViewModel(groupChatRoom);
		}

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new AddPinBoardMessage(VM.CurrentGroupChat));
        }

        private void EditBtn_Clicked(object sender, EventArgs e)
        {

        }
    }
}