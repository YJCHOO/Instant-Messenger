using Pal.Service;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class AuthenticationViewModel : INotifyPropertyChanged
    {

        public String EmailText { get; set; }
        public String PassText { get; set; }
        public String ErrorMsg { get; set; }
        public ICommand OnLoginCommand { get; set; }
        public ICommand OnRegisterCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private string TempUser;
        private readonly Validator validator = new Validator();


        public AuthenticationViewModel()
        {

            OnLoginCommand = new Command(async() =>
            {
                if(!IsValidEmailPass()){
                    return;  
                }

                try
                {
                    TempUser = await DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPass(EmailText, PassText);
                    OnPropertyChanged("TempUser");
                    UserSetting.UserToken = TempUser;
                }
                catch (Exception Ex) {
                    FirebaseException(Ex.ToString());
                }
            });

            OnRegisterCommand = new Command(async () =>
            {
                if (!IsValidEmailPass())
                {
                    return;
                }

                try
                {
                    TempUser = await DependencyService.Get<IFirebaseAuthenticator>().RegisterWithEmailPassword(EmailText, PassText);
                    OnPropertyChanged("TempUser");
                    UserSetting.UserToken = TempUser;
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
            else if (ExceptionMsg.Contains("FirebaseAuthUserCollisionException")) {
                ErrorMsg = "The email address is already registered. Please login with this email address.";
            }
            OnPropertyChanged("ErrorMsg");
        }

        private bool IsValidEmailPass() {
            var TempStr = validator.ValidateEmailPass(EmailText, PassText);
            if (String.IsNullOrEmpty(TempStr))
            {
                ErrorMsg = "";
                OnPropertyChanged("ErrorMsg");
                return true;
            }
            else
            {
                ErrorMsg = TempStr;
                OnPropertyChanged("ErrorMsg");
                return false;
            }
        }

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }

    }
}
