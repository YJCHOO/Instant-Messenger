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
	public partial class IncomingDocAttachmentViewCell : ViewCell
	{
		public IncomingDocAttachmentViewCell ()
		{
			InitializeComponent ();
		}

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var uri = DocUri.Text;
            await Application.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(uri));
        }
    }
}