using Pal.Model;
using Pal.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MomentSendTo : ContentPage
	{
        SocialViewModel VM;
		public MomentSendTo (Moment _Moment)
		{
			InitializeComponent ();
            VM = new SocialViewModel(_Moment);
            BindingContext = VM;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FriendsListView.BeginRefresh();
            VM.InitialUser();
            FriendsListView.EndRefresh();
        }
    }
}