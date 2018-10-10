using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Firebase;

namespace Pal.Droid
{
    [Activity(Label = "Pal", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static FirebaseApp app;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            InitiFirebaseAuth();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private void InitiFirebaseAuth() {
            var options = new FirebaseOptions.Builder()
                .SetApplicationId("")
                .SetApiKey("")
                .SetDatabaseUrl("")
                .SetProjectId("")
                .Build();

            if (app == null) {
                app = FirebaseApp.InitializeApp(this,options, "Pal");
            }

        }

    }
}
