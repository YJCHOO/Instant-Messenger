using Pal.Service;
using Pal.View;
using Pal.View.Authentication;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Pal
{
    public partial class App : Application
    {
        public static string User = "Rendy";

        public App()
        {

            InitializeComponent();


            //if (!UserSetting.IsUserSet)
            //{
                MainPage = new NavigationPage(new AuthenticationPage());

            //}
            //else
            //{
            //    MainPage = new NavigationPage(new ChatsPage());
            //}
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
