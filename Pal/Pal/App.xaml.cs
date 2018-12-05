using Pal.Model;
using Pal.Service;
using Pal.View;
using Pal.View.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Pal
{
    public partial class App : Application
    {

        public static List<User> FriendsList { get; set; } = new List<User>();
        public static User searchResult = new User();

        public App()
        {
            InitializeComponent();



            if (UserSetting.IsUserEmail && UserSetting.IsUserName)
            {

                MainPage = new NavigationPage(new MainPage());

            }
            else
            {
                MainPage = new NavigationPage(new AuthenticationPage());
            }
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
