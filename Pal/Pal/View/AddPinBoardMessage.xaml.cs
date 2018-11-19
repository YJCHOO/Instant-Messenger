using System;
using Pal.Model;
using Pal.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPinBoardMessage : ContentPage
	{
        PinBoardViewModel VM;

		public AddPinBoardMessage (GroupChatRoom groupChatRoom)
		{
            InitializeComponent();
            BindingContext = VM = new PinBoardViewModel(groupChatRoom);
		}

        private async void AddAttachment_Clicked(object sender, EventArgs e)
        {
            var attachment = await VM.AddAttachment();
        }

        private void Post_Clicked(object sender, EventArgs e)
        {

        }
    }
}