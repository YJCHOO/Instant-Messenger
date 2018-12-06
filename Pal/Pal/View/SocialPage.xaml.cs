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
	public partial class SocialPage : ContentPage
	{
        SocialViewModel VM;

		public SocialPage ()
		{
			InitializeComponent ();
            VM = new SocialViewModel();
            BindingContext = VM;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MomentsList.BeginRefresh();
            VM.InitialMoments();
            MomentsList.EndRefresh();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            VM.ClearAllMoment();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(((Image)sender).Source.ToString()));
        }
    }
}