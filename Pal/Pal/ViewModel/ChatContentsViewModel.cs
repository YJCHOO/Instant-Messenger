using Pal.Model;
using Pal.Service;
using Plugin.Connectivity;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace Pal.ViewModel
{
    public class ChatContentsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public string TextToSend { get; set; }
        public ICommand OnSendCommand { get; set; }
        public ChatRoom ChatRoom;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Attachment { get; set; }
        public string FileName { get; set; }
        public FileData PickedFileData = null;
        

        public ChatContentsViewModel(object objChatRoom)
        {
            SetChatRoomType(objChatRoom);

            OnSendCommand = new Command(async () =>
            {
                if (!CrossConnectivity.Current.IsConnected) {
                    await App.Current.MainPage.DisplayAlert("No Internet Connection", "Message not able to send, Please check your internet connection and try again.", "Ok");
                    return;
                }

                if (!string.IsNullOrEmpty(TextToSend) || PickedFileData != null)
                {
                    
                    Attachment attachmentType = new Attachment() ;
                    if (PickedFileData != null)
                    {

                        attachmentType = await UploadFile(PickedFileData);
                    }

                    Message message = new Message(UserSetting.UserEmail, UserSetting.UserName, this.TextToSend, attachmentType.AttachmentUri,attachmentType.FileName);
                    TextToSend = string.Empty;
                    OnPropertyChanged("TextToSend");
                    DependencyService.Get<IFirebaseDatabase>().SetMessage(ChatRoom, message);
                    
                }

            });
        }

        private void SetChatRoomType(object objChatRoom)
        {
            if (objChatRoom.GetType() == typeof(IndividualChatRoom))
                ChatRoom = (IndividualChatRoom)objChatRoom;
            else
                ChatRoom = (GroupChatRoom)objChatRoom;
        }

        public async Task OnAppearing() {
           var NewMessages =  await DependencyService.Get<IFirebaseDatabase>().GetMessage(ChatRoom.RoomID);
            if (NewMessages != null)
            {
                Messages.Clear();
                Messages = NewMessages;
                this.OnPropertyChanged("Messages");
            }
        }

        public async Task OnDisappearing() {

            DependencyService.Get<IFirebaseDatabase>().ClearMessages();
            var ForEachMessage = Messages;
            foreach (Message tempMessage in ForEachMessage) {
                if (tempMessage.IsDestruct)
                {
                    var status = await DependencyService.Get<IFirebaseDatabase>().DestructMessage(tempMessage.MessageId);

                    if (!status)
                    {
                        await App.Current.MainPage.DisplayAlert("Something happen....", "Message unable to destruct", "Ok");
                        break;
                    }
                }
            }
        }

        public async Task UpdateRoomDestructStatus(bool destructStatus) {
            var UpdateStatus =  await DependencyService.Get<IFirebaseDatabase>().SetRoomDestruct(ChatRoom.RoomID, destructStatus);
            if (UpdateStatus != null) {
                await App.Current.MainPage.DisplayAlert("Something happen....", UpdateStatus, "Ok");
            }
        }


        public async Task<bool> PickAndShowFile() {
            //Check Permission
            if (!await this.CheckPermissionsAsync())
            {
                return false;
            }

            string CheckType=null;
            var pickedFile = await CrossFilePicker.Current.PickFile();
            if (pickedFile == null){ return false; }
            else { 
                CheckType = IsSupportedType(pickedFile.FileName);

                if (CheckType == null)
                {
                    await App.Current.MainPage.DisplayAlert("Something happen....", "Selected file format not supported", "Ok");
                    return false;
                }

                switch (CheckType) {
                    case "Img":
                        Attachment = pickedFile.FilePath;
                        break;

                    case "Video":
                        Attachment = "round_movie_black_48.png";
                        break;

                    case "PDF":
                        Attachment = "round_insert_drive_file_black_48.png";
                        break;
                }
                FileName = "Attachment: " + pickedFile.FileName;

                this.OnPropertyChanged("Attachment");
                this.OnPropertyChanged("FileName");
                PickedFileData = pickedFile;
                return true;
            }
        }

        private string IsSupportedType(string FileType) {

            string Type = null;

            if (FileType.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                   FileType.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                Type = "Img";
            else if (FileType.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
                Type = "Video";
            else if (FileType.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                Type = "PDF";
            return Type;
        }

        private async Task<Attachment> UploadFile(FileData FileStream) {
               var url =  await DependencyService.Get<IFirebaseStorage>().UploadFile(FileStream);
            return url;
        }

        private async Task<bool> CheckPermissionsAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await App.Current.MainPage.DisplayAlert("Xamarin.Forms Sample", "Storage permission is needed for file picking", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);

                    if (results.ContainsKey(Permission.Storage))
                    {
                        status = results[Permission.Storage];
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await App.Current.MainPage.DisplayAlert("Xamarin.Forms Sample", "Storage permission was denied.", "OK");
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    


    public void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }
    }
}
