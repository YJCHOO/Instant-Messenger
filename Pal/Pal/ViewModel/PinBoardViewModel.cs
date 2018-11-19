using Pal.Model;
using Pal.Service;
using Pal.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class PinBoardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Board> PinBoardMessages { get; set; }
        public GroupChatRoom CurrentGroupChat { get; set; }
        public ICommand OnAddCommand { get; set; }
        public ICommand OnEditCommand { get; set; }
        public bool IsAdmin { get; set; }
        public string BoardMessageTitle { get; set; }
        public string Description { get; set; }


        public string FileName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public PinBoardViewModel(GroupChatRoom groupChatRoom) {

            CurrentGroupChat = groupChatRoom;

            if (CurrentGroupChat.Admin.Equals(UserSetting.UserEmail)) {
                IsAdmin = true;
            }

            OnAddCommand = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new AddPinBoardMessage(CurrentGroupChat));
            });


            OnEditCommand = new Command(() =>
            {

            });
        }
        public void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        public async Task<Attachment> AddAttachment()
        {
            FilePicker _FilePicker = FilePicker.GetInstance();
            Attachment attachment = null;
            string Thumbnail = "";
            if (!await _FilePicker.CheckPermission()) {
                return null;
            }

            var pickedFile = await _FilePicker.PickAndShowFile();
            if (pickedFile != null)
            {
                string CheckType = _FilePicker.IsSupportedType(_FilePicker.IsSupportedType(pickedFile.FileName));
                if (CheckType == null) {

                    await App.Current.MainPage.DisplayAlert("Something happen....", "Selected file format not supported", "Ok");
                    return null;
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
                attachment = new Attachment(pickedFile.FileName,Thumbnail,pickedFile);
            
                OnPropertyChanged("FileName");
            }
            return attachment;
        }
        


    }
}
