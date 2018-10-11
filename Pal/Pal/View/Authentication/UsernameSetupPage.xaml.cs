using Pal.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View.Authentication
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UsernameSetupPage : ContentPage
	{
		public UsernameSetupPage ()
		{
			InitializeComponent ();
            this.BindingContext = new UsernameSetupPageViewModel();

        }
	}
}