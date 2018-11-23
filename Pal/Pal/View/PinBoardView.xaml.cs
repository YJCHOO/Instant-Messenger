using Pal.Model;
using Pal.ViewModel;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PinBoardView : ContentPage
	{
        private PinBoardViewModel VM { get; set; }
        public PinBoardView (GroupChatRoom groupChatRoom)
		{   
			InitializeComponent ();

            BindingContext = VM = new PinBoardViewModel(groupChatRoom);
		}

        protected override async void OnAppearing()
        {
            
            await VM.InitialBoardListAsync();
        }

       

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new AddPinBoardMessage(VM.CurrentGroupChat));
        }
    }
}