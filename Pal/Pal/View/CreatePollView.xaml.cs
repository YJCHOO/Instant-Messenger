using Pal.ViewModel;
using Pal.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePollView : ContentPage
	{
        private PollViewModel VM;

		public CreatePollView (GroupChatRoom groupChatRoom)
		{
			InitializeComponent ();
            BindingContext = VM = new PollViewModel(groupChatRoom);
		}
	}
}