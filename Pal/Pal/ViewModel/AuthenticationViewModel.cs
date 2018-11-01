using Pal.Service;
using Pal.View;
using Pal.View.Authentication;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    public class AuthenticationViewModel : INotifyPropertyChanged
    {

        public string EmailText { get; set; }
        public string PassText { get; set; }
        public string Username { get; set; }
        public string AlertMsg { get; set; }
        public ICommand OnNextCommand { get; set; }
        public ICommand OnLoginCommand { get; set; }
        public ICommand OnRegisterCommand { get; set; }
        private readonly Validator validator = new Validator();

        public event PropertyChangedEventHandler PropertyChanged;

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
                    UserSetting.UserEmail = await DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPass(EmailText, PassText);
                    UserSetting.UserName = await DependencyService.Get<IFirebaseDatabase>().GetUsername(EmailText);
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
                    UserSetting.UserEmail = await DependencyService.Get<IFirebaseAuthenticator>().RegisterWithEmailPassword(EmailText, PassText);
                    DependencyService.Get<IFirebaseDatabase>().SetUser(EmailText, "");
                    await App.Current.MainPage.Navigation.PushAsync(new UsernameSetupPage());
                }
                catch (Exception Ex) {
                    FirebaseException(Ex.ToString());
                }
            });

            if (!string.IsNullOrEmpty(UserSetting.UserName))
            {
                Username = UserSetting.UserName;

            }

            OnNextCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    await App.Current.MainPage.DisplayAlert("Something happen....", "Name can't be empty", "Ok");
                    return;
                }

                try
                {
                    DependencyService.Get<IFirebaseDatabase>().UpdateUser(UserSetting.UserEmail, Username);
                    UserSetting.UserName = Username;

                    App.Current.MainPage = new NavigationPage(new ChatsPage());
                }
                catch (Exception Ex)
                {
                    Debug.Write(Ex.Message);

                }
            });

        }

        private void FirebaseException(string ExceptionMsg) {
            if (ExceptionMsg.Contains("FirebaseNetworkException"))
            {
                AlertMsg = "No internet Connection. Please Try again.";

            }
            else if (ExceptionMsg.Contains("FirebaseAuthInvalidCredentialsException"))
            {
                AlertMsg = "No internet Connection. Please Try again.";
            }
            else if (ExceptionMsg.Contains("FirebaseAuthInvalidUserException"))
            {
                AlertMsg = "Email not found. Please register first.";

            }
            else if (ExceptionMsg.Contains("FirebaseAuthUserCollisionException"))
            {
                AlertMsg = "The email address is already registered. Please login with this email address.";
            }
            else
            {
                AlertMsg = ExceptionMsg;
            }
            DisplayAlert();
            
        }

        private bool IsValidEmailPass() {
            var TempStr = validator.ValidateEmailPass(EmailText, PassText);
            if (String.IsNullOrEmpty(TempStr))
            {
                AlertMsg = "";
                return true;
            }
            else
            {
                AlertMsg = TempStr;
                return false;
            }
        }

        private async void DisplayAlert() {
            if(!string.IsNullOrEmpty(AlertMsg))
            await App.Current.MainPage.DisplayAlert("Something happen....", AlertMsg, "Ok");
        }
        
        

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }
    }
}
