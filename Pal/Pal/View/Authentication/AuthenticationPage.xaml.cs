using Pal.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationPage : ContentPage
    {

        public AuthenticationPage()
        {
            InitializeComponent();
            this.BindingContext = new AuthenticationViewModel();
        }



    }
}