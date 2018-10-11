using Pal.Service;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class UsernameSetupPageViewModel : INotifyPropertyChanged
    {
        public string Username { set; get; }
        public ICommand OnNextCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public UsernameSetupPageViewModel() {


            if (!string.IsNullOrEmpty(UserSetting.UserName)) {
                Username = UserSetting.UserName;

            }

            OnNextCommand = new Command(async() =>
            {
                if (string.IsNullOrWhiteSpace(Username)) {
                    await App.Current.MainPage.DisplayAlert("Something happen....", "Name can't be empty", "Ok");
                    return;
                }

                try
                {
                    DependencyService.Get<IFirebaseDatabase>().UpdateUser(UserSetting.UserEmail, Username);
                    UserSetting.UserName = Username;
                }
                catch (Exception Ex) {
                    Debug.Write(Ex.Message);

                }
            });
        }


        public void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

    }
}
