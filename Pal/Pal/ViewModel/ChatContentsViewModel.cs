using Pal.Model;
using Pal.Service;
using Plugin.Connectivity;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ChatRoom ChatRoom = new ChatRoom();
        public GroupChatRoom GroupChatRoom = new GroupChatRoom();
        public bool AttachmentSection { get; set; } = false;
        public bool Uploading { get; set; } = false;
        public Attachment Attachment { get; set; } = new Attachment();
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatContentsViewModel(object objChatRoom)
        {
            SetChatRoomType(objChatRoom);

            OnSendCommand = new Command(async () =>
            {
                if (!CrossConnectivity.Current.IsConnected) {
                    await App.Current.MainPage.DisplayAlert("No Internet Connection", "Message not able to send, Please check your internet connection and try again.", "Ok");
                    return;
                }

                if (!string.IsNullOrEmpty(TextToSend) || Attachment._FileData != null)
                {
                    
                    Attachment attachmentType = new Attachment() ;
                    if (Attachment._FileData != null)
                    {
                        Uploading = true;
                        AttachmentSection = false;
                        OnPropertyChanged("Uploading");
                        OnPropertyChanged("AttachmentSection");
                        attachmentType = await UploadFile(Attachment._FileData);
                        Uploading = false;
                        OnPropertyChanged("Uploading");
                    }

                    Message message = new Message(UserSetting.UserEmail, UserSetting.UserName, this.TextToSend, attachmentType.AttachmentUri,attachmentType.FileName);
                    
                    await DependencyService.Get<IFirebaseDatabase>().SetMessage(ChatRoom, message);

                    TextToSend = string.Empty;
                    AttachmentSection = false;
                    RemoveAttachment();
                    OnPropertyChanged("TextToSend");
                    OnPropertyChanged("AttachmentSection");
                }

            });
        }

        private void SetChatRoomType(object objChatRoom)
        {
            if (objChatRoom.GetType() == typeof(IndividualChatRoom))
                ChatRoom = (IndividualChatRoom)objChatRoom;
            else {
                ChatRoom = (GroupChatRoom)objChatRoom;
                GroupChatRoom = (GroupChatRoom)objChatRoom;
            }
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

            FilePicker _FilePicker = FilePicker.GetInstance();
            string thumbnail = null;


            if (!await _FilePicker.CheckPermission()) {
                return false;
            }

            string CheckType=null;
            var pickedFile = await _FilePicker.PickAndShowFile();
            if (pickedFile == null){ return false; }
            else { 
                CheckType = _FilePicker.IsSupportedType(pickedFile.FileName);

                if (CheckType == null)
                {
                    await App.Current.MainPage.DisplayAlert("Something happen....", "Selected file format not supported", "Ok");
                    return false;
                }

                switch (CheckType) {
                    case "Img":
                        thumbnail = pickedFile.FilePath;
                        break;

                    case "Video":
                        thumbnail = "round_movie_black_48.png";
                        break;

                    case "PDF":
                        thumbnail = "round_insert_drive_file_black_48.png";
                        break;
                }
                Attachment.FileName=pickedFile.FileName;
                Attachment.Thumbnail = thumbnail;
                Attachment._FileData = pickedFile;
                OnPropertyChanged("Attachment");
                return true;
            }
        }


        private async Task<Attachment> UploadFile(FileData FileStream) {
            Attachment url =  await DependencyService.Get<IFirebaseStorage>().UploadFile(FileStream);
            
            return url;
        }

        public void DisplayAttachment() {
            AttachmentSection = true;
            OnPropertyChanged("AttachmentSection");

        }

        public void RemoveAttachment() {
            Attachment = new Attachment();
            AttachmentSection = false;
            OnPropertyChanged("AttachmentSection");
        } 


    public void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }
    }
}
