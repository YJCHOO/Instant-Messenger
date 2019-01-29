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
        public ICommand OnClickAttachment { get; set; }
        public ObservableCollection<SelectableData<User>> SendTo { get; set; } = new ObservableCollection<SelectableData<User>>();
        public ObservableCollection<Moment> Moments { get; set; } = new ObservableCollection<Moment>();
        public string Description { get; set; }
        public Attachment _Attachment { get; set; } = new Attachment();
        public bool IsOnAttachmentSection { get; set; } = false;
        public bool IsFriendsListEmpty { get; set; } = false;
        public bool IsFriendsNotEmpty { get; set; } = true;
        public bool IsProcessing { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsMomentsListEmpty { get; set; } = false;

        public SocialViewModel() {
            OnAddMoments = new Command(async() => {
            await App.Current.MainPage.Navigation.PushAsync(new AddMoments() );
            });

            OnPickFileCommand = new Command(async () => {

                FilePicker filePicker = FilePicker.GetInstance();
                string CheckType = null;
                string thumbnail = null;
                 
                if (!await filePicker.CheckPermission()) {
                    return;
                }
               
                    var pickedFile = await filePicker.PickAndShowFile();
                    if (pickedFile == null) { return; }
                    else
                    {
                            CheckType = filePicker.IsSupportedMomentType(pickedFile.FileName);
                        if (CheckType == null)
                        {
                            await App.Current.MainPage.DisplayAlert("Something wrong....",
                                "File format not supported.\nSupport format: \n*.MP4 \n*.jpg*\n*.png",
                                "Ok");
                            return;
                        }

                        switch (CheckType) {
                            case "Img":
                            thumbnail = pickedFile.FilePath;
                                break;
                            case "Video":
                            thumbnail = "round_movie_black_48.png";
                                break;
                        }
                    }
                IsOnAttachmentSection = true;
                _Attachment.FileName = pickedFile.FileName;
                _Attachment._FileData = pickedFile;
                _Attachment.Thumbnail = thumbnail;
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
                Moment _Moment = new Moment(UserSetting.UserEmail,UserSetting.UserName, Description,_Attachment);
                await App.Current.MainPage.Navigation.PushAsync(new MomentSendTo(_Moment));
            });

            OnDeleteCommand = new Command(() =>
            {
                _Attachment = null;
                IsOnAttachmentSection = false;
                OnPropertyChanged("IsOnAttachmentSection");
            });

            OnClickAttachment = new Command<string>(async(string uri) => {
                await App.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(uri));
            });

        }


        public SocialViewModel(Moment _Moment) {
            OnPostCommand = new Command(async() => {

                if (SendTo.Count == 0) {
                    await App.Current.MainPage.DisplayAlert("Something wrong...", "Please select one friend to send.", "Ok");
                    return;
                }

                IsProcessing = true;
                OnPropertyChanged("IsProcessing");
                var TempSelectedUser = new Dictionary<string, bool>();
                foreach (SelectableData<User> selectedUser in SendTo) {
                    if (selectedUser.Selected) {
                        TempSelectedUser.Add(selectedUser.Data.Email.Replace(".",":"),true);
                    }
                }
                _Moment.Receiver = TempSelectedUser;

                try
                {
                    if (!string.IsNullOrEmpty(_Moment._Attachment._FileData.FileName))
                    {
                        _Moment._Attachment.AttachmentUri = await DependencyService.Get<IFirebaseStorage>().UploadMoments(_Moment._Attachment._FileData);
                        if (string.IsNullOrEmpty(_Moment._Attachment.AttachmentUri))
                        {
                            await App.Current.MainPage.DisplayAlert("Something wrong....", "Attachment unable to upload.Please try again.", "Ok");
                            return;
                        }
                    }
                }
                catch (NullReferenceException) { }

                bool status = await DependencyService.Get<IFirebaseDatabase>().CreateMoment(_Moment);
                if (status)
                {
                    await App.Current.MainPage.DisplayAlert("Done.", "Moment posted.", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong...", "Moment unable to post, please try again", "Ok");
                }
                IsProcessing = false;
                OnPropertyChanged("IsProcessing");
                await App.Current.MainPage.Navigation.PopToRootAsync();
            });
        }


        public async void InitialUser() {
            List<User> friendsList = new List<User>();
            friendsList = await DependencyService.Get<IFirebaseDatabase>().GetFriendsList();
            if (friendsList.Count == 0)
            {
                IsFriendsListEmpty = true;
                IsFriendsNotEmpty = false;
            }
            else
            {
                IsFriendsListEmpty = false;
                IsFriendsNotEmpty = true;
            }

            foreach (User user in friendsList) {

                SendTo.Add(new SelectableData<User>(user,false));
            }
            OnPropertyChanged("SendTo");
            OnPropertyChanged("IsFriendsListEmpty");
            OnPropertyChanged("IsFriendsNotEmpty");
        }

        public async void InitialMoments() {

            Moments = await DependencyService.Get<IFirebaseDatabase>().GetMomentsList();
            if (Moments.Count == 0)
            {
                IsMomentsListEmpty = true;
            }
            else
            {
                IsMomentsListEmpty = false;
            }
            OnPropertyChanged("IsMomentsListEmpty");
            OnPropertyChanged("Moments");
        }

        public void ClearAllMoment() {
            DependencyService.Get<IFirebaseDatabase>().ClearAllMoments();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
