using Pal.Service;
using Pal.View.Authentication;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class AuthenticationViewModel
    {

        public string EmailText { get; set; }
        public string PassText { get; set; }
        public string Username { get; set; }
        public String ErrorMsg { get; set; }
        public ICommand OnLoginCommand { get; set; }
        public ICommand OnRegisterCommand { get; set; }
        private string TempUser;
        private readonly Validator validator = new Validator();


        public AuthenticationViewModel()
        {

            OnLoginCommand = new Command(async() =>
            {
                if(!IsValidEmailPass()){
                    DisplayAlert();
                    return;  
                }

                try
                {
                    TempUser = await DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPass(EmailText, PassText);
                    DependencyService.Get<IFirebaseDatabase>().GetUser(EmailText);
                    UserSetting.UserEmail = TempUser;
                    await App.Current.MainPage.Navigation.PushAsync(new UsernameSetupPage());
                }
                catch (Exception Ex) {
                    FirebaseException(Ex.ToString());
                }
            });

            OnRegisterCommand = new Command(async () =>
            {
                if (!IsValidEmailPass())
                {
                    DisplayAlert();
                    return;
                }

                try
                {
                    TempUser = await DependencyService.Get<IFirebaseAuthenticator>().RegisterWithEmailPassword(EmailText, PassText);
                    UserSetting.UserEmail = TempUser;
                    Debug.Write(TempUser);
                    DependencyService.Get<IFirebaseDatabase>().SetUser(EmailText, "");
                    await App.Current.MainPage.Navigation.PushAsync(new UsernameSetupPage());

                }
                catch (Exception Ex) {
                    FirebaseException(Ex.ToString());
                }
            });
        }

        private void FirebaseException(string ExceptionMsg) {
            if (ExceptionMsg.Contains("FirebaseNetworkException"))
            {
                ErrorMsg = "No internet Connection. Please Try again.";

            }
            else if (ExceptionMsg.Contains("FirebaseAuthInvalidCredentialsException"))
            {
                ErrorMsg = "No internet Connection. Please Try again.";
            }
            else if (ExceptionMsg.Contains("FirebaseAuthInvalidUserException"))
            {
                ErrorMsg = "Email not found. Please register first.";

            }
            else if (ExceptionMsg.Contains("FirebaseAuthUserCollisionException"))
            {
                ErrorMsg = "The email address is already registered. Please login with this email address.";
            }
            else
            {
                ErrorMsg = ExceptionMsg;
            }
            DisplayAlert();
            
        }

        private bool IsValidEmailPass() {
            var TempStr = validator.ValidateEmailPass(EmailText, PassText);
            if (String.IsNullOrEmpty(TempStr))
            {
                ErrorMsg = "";
                return true;
            }
            else
            {
                ErrorMsg = TempStr;
                return false;
            }
        }

        private async void DisplayAlert() {
            if(!string.IsNullOrEmpty(ErrorMsg))
            await App.Current.MainPage.DisplayAlert("Something happen....", ErrorMsg, "Ok");
        }


    }
}
