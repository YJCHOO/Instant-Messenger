using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View.CustomViewCell
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutgoingDocAttachmentViewCell : ViewCell
	{
		public OutgoingDocAttachmentViewCell ()
		{
			InitializeComponent ();
		}

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var uri = DocUri.Text.ToString();
            await Application.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(uri));
        }
    }
}