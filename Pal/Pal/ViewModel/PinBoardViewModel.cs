using Pal.Model;
using Pal.Service;
using Pal.View;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;

namespace Pal.ViewModel
{
    class PinBoardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Board> PinBoardMessages { get; set; }
        public GroupChatRoom CurrentGroupChat { get; set; } = new GroupChatRoom();
        public ICommand OnAddCommand { get; set; }
        public ICommand OnEditCommand { get; set; }
        public ICommand OnAddAttachmentCommand { get; set; }
        public ICommand OnRemoveAttachmentCommand { get; set; }
        public ICommand OnPostCommand { get; set; }
        public ICommand OnClickAttachment { get; set; }
        public ICommand OnContextActionCommand { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsAttach { get; set; } = false;
        public string BoardMessageTitle { get; set; }
        public string Description { get; set; }
        public Attachment BoardMessageAttachment { get; set; } = new Attachment();
        public bool IsEmpty { get; set; } = true;
        public event PropertyChangedEventHandler PropertyChanged;

        public PinBoardViewModel(GroupChatRoom groupChatRoom) {

            CurrentGroupChat = groupChatRoom;
            if (CurrentGroupChat.Admin.Equals(UserSetting.UserEmail))
            {
                IsAdmin = true;
            }
            else { IsAdmin = false; }

            OnRemoveAttachmentCommand = new Command(() => {
                RemoveAttachment();
            });

            OnAddAttachmentCommand = new Command(async() => { 
                await AddAttachment();   
            });

            OnAddCommand = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new AddPinBoardMessage(CurrentGroupChat));
            });

            OnPostCommand = new Command(async () =>
            {

                if (string.IsNullOrWhiteSpace(BoardMessageTitle))
                {

                    await App.Current.MainPage.DisplayAlert("Something wrong....", "Need a Title for this message.", "OK");
                    return;
                }
                

                if (!string.IsNullOrEmpty(BoardMessageAttachment.FileName)) {
                    BoardMessageAttachment= await DependencyService.Get<IFirebaseStorage>().UploadFile(BoardMessageAttachment._FileData);
                }

                User MessageByUser = new User(UserSetting.UserEmail, UserSetting.UserName);
                Board boardMessage = new Board("",BoardMessageTitle, Description, BoardMessageAttachment, MessageByUser, new DateTime());
                var Status = await DependencyService.Get<IFirebaseDatabase>().AddPinBoardMessage(CurrentGroupChat.RoomID,boardMessage);
                if (!Status)
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong.....", "Message not able to Pin,Please try again", "OK");
                }
                else {

                    await App.Current.MainPage.DisplayAlert("Done", "Posted", "OK");
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            });

            OnClickAttachment = new Command<string>(async (string parameter) =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new WebViewAttachment(parameter));
            });

            OnContextActionCommand = new Command<string>(async(string parameter) => {
                if (!groupChatRoom.Admin.Equals(UserSetting.UserEmail)) {
                    await App.Current.MainPage.DisplayAlert("You are not admin", "Only admin can do it", "Ok");
                    return;
                }

                var status = await DependencyService.Get<IFirebaseDatabase>().RemovePinBoardMessage(parameter);

                if (status)
                {
                    await App.Current.MainPage.DisplayAlert("Done","Message deleted.","Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong ....","Message unable to delete, Please try again.","Ok");
                }
            });

        }

        protected virtual void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }

        public async Task<Attachment> UploadAttachment(FileData fileData) {

            var UploadedAttachment = await DependencyService.Get<IFirebaseStorage>().UploadFile(fileData);
            return UploadedAttachment;

        }

        public async Task AddAttachment()
        {
            FilePicker _FilePicker = FilePicker.GetInstance();
            string Thumbnail = "";
            if (!await _FilePicker.CheckPermission()) {
                return;
            }

            var pickedFile = await _FilePicker.PickAndShowFile();
            if (pickedFile != null)
            {
                string CheckType = _FilePicker.IsSupportedType(pickedFile.FileName);
                if (CheckType == null) {

                    await App.Current.MainPage.DisplayAlert("Something happen....", "Selected file format not supported", "Ok");
                    return;
                }
                
                switch (CheckType) {
                    case "Img":
                        Thumbnail= pickedFile.FilePath;
                        break;

                    case "Video":
                        Thumbnail = "round_movie_black_48.png";
                        break;

                    case "PDF":
                        Thumbnail = "round_insert_drive_file_black_48.png";
                        break;
                }
                BoardMessageAttachment = new Attachment(pickedFile.FileName, Thumbnail, pickedFile);
                Debug.Write(BoardMessageAttachment.Thumbnail);
                IsAttach = true;
                
                OnPropertyChanged("IsAttach");
                OnPropertyChanged("BoardMessageAttachment");
            }
        }

        public async Task InitialBoardListAsync() {

            
            PinBoardMessages =  await DependencyService.Get<IFirebaseDatabase>().GetAllPinBoardMessage(CurrentGroupChat.RoomID);
            if (PinBoardMessages!=null&& PinBoardMessages.Count != 0)
            {
                PinBoardMessages = new ObservableCollection<Board>(PinBoardMessages.Reverse());
                IsEmpty = false;
            }
            OnPropertyChanged("IsEmpty");
            OnPropertyChanged("PinBoardMessages");

        }

        public void OnDisappearing()
        {
            DependencyService.Get<IFirebaseDatabase>().ClearAllPinBoardMessage();
        }

        public void RemoveAttachment() {
            BoardMessageAttachment = new Attachment();
            IsAttach = false;
            OnPropertyChanged("IsAttach");
            OnPropertyChanged("BoardMessageAttachment");
        }
    }
}
