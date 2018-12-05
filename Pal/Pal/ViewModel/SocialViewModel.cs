using Pal.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Pal.Service;
using Pal.Model;
using Plugin.FilePicker.Abstractions;
using System.Collections.ObjectModel;

namespace Pal.ViewModel
{
    class SocialViewModel : INotifyPropertyChanged
    {
        public ICommand OnAddMoments { get; set; }
        public ICommand OnSendToCommand { get; set; }
        public ICommand OnPickFileCommand { get; set; }
        public ICommand OnPostCommand { get; set; }
        public ICommand OnDeleteCommand { get; set; }
        public ObservableCollection<SelectableData<User>> SendTo { get; set; } = new ObservableCollection<SelectableData<User>>();
        public string Description { get; set; }
        public Attachment _Attachment { get; set; } = new Attachment();
        public bool IsOnAttachmentSection { get; set; } = false;
        public bool IsFriendsListEmpty { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public SocialViewModel() {
            OnAddMoments = new Command(async() => {
            await App.Current.MainPage.Navigation.PushAsync(new AddMoments() );
            });

            OnPickFileCommand = new Command(async () => {

                FilePicker filePicker = FilePicker.GetInstance();
                var status=  await filePicker.CheckPermission();
                if (!status) {
                    return;
                }
                _Attachment._FileData = await filePicker.PickAndShowFile();
                string CheckType = filePicker.IsSupportedMomentType(_Attachment._FileData.FileName);
                if (CheckType == null)
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong....",
                        "File format not supported.\nSupport format: \n*.MP4 \n*.jpg*\n*.png",
                        "Ok");
                    return;
                }

                if (CheckType.Equals("Img"))
                {
                    _Attachment.Thumbnail = _Attachment._FileData.FilePath;
                }
                else
                    _Attachment.Thumbnail = "";

                IsOnAttachmentSection = true;
                OnPropertyChanged("IsOnAttachmentSection");
                OnPropertyChanged("_Attachment");

            });

            OnSendToCommand = new Command(async () => {
                try
                {
                    if (string.IsNullOrWhiteSpace(Description) && string.IsNullOrEmpty(_Attachment.FileName))
                    {
                        await App.Current.MainPage.DisplayAlert("Something wrong...", "Must write something or attach some photo/video.", "Ok");
                        return;
                    }
                }
                catch (NullReferenceException ) {
                }
                Moment _Moment = new Moment(UserSetting.UserEmail, Description,_Attachment);
                await App.Current.MainPage.Navigation.PushAsync(new MomentSendTo(_Moment));
            });

            OnDeleteCommand = new Command(() =>
            {
                _Attachment = null;
                IsOnAttachmentSection = false;
                OnPropertyChanged("IsOnAttachmentSection");
            });
        }


        public SocialViewModel(Moment _Moment) {
            OnPostCommand = new Command(async() => {
                var TempSelectedUser = new Dictionary<string, bool>();
                foreach (SelectableData<User> selectedUser in SendTo) {
                    if (selectedUser.Selected) {
                        TempSelectedUser.Add(selectedUser.Data.Email.Replace(".",":"),true);
                    }
                }
                _Moment._Attachment.AttachmentUri = await DependencyService.Get<IFirebaseStorage>().UploadMoments(_Moment._Attachment._FileData);
                if (string.IsNullOrEmpty(_Moment._Attachment.AttachmentUri)) {
                    await App.Current.MainPage.DisplayAlert("Something wrong....","Attachment unable to upload.Please try again.","Ok");
                    return;
                }
                _Moment.Receiver = TempSelectedUser;

                bool status = await DependencyService.Get<IFirebaseDatabase>().CreateMoment(_Moment);
                if (status)
                {
                    await App.Current.MainPage.DisplayAlert("Done.", "Moment posted.", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong...", "Moment unable to post, please try again", "Ok");
                }
            });
        }

        public async void InitializeUser() {
            List<User> friendsList = new List<User>();
            friendsList = await DependencyService.Get<IFirebaseDatabase>().GetFriendsList();
            if (friendsList.Count == 0)
            {
                IsFriendsListEmpty = true;
            }
            else { IsFriendsListEmpty = false; }

            foreach (User user in friendsList) {

                SendTo.Add(new SelectableData<User>(user,false));
            }
            OnPropertyChanged("SendTo");
            OnPropertyChanged("IsFriendsListEmpty");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
