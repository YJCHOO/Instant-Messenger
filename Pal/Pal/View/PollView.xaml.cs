using Pal.ViewModel;
using Pal.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PollView : ContentPage
	{
        PollViewModel VM;


		public PollView (GroupChatRoom groupChatRoom)
		{
			InitializeComponent ();
            VM = new PollViewModel(groupChatRoom);
            BindingContext = VM;
		}

        protected override async void OnAppearing()
        {
            
           await VM.InitialPoll();

        }
        

        private async void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            OptionsList.IsEnabled = false;
            await VM.UpadateVoteResult();
        }
    }
}