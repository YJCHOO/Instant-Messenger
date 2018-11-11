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
	public partial class IncomingVideoAttachementViewCell : ViewCell
	{
		public IncomingVideoAttachementViewCell ()
		{
			InitializeComponent ();
		}

        private async void StackLayout_Tapped(object sender, EventArgs e)
        {
            var uri = VideoUri.Text;
            await Application.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(uri));
        }
    }
}