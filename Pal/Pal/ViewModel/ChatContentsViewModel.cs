using Pal.Model;
using Pal.Service;
using System;
using System.Collections.Generic;
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
        public ChatRoom ChatRoom;
        public event PropertyChangedEventHandler PropertyChanged;


        public ChatContentsViewModel(object objChatRoom)
        {
            if (objChatRoom.GetType() == typeof(IndividualChatRoom))
                ChatRoom = (IndividualChatRoom)objChatRoom;
            else 
                ChatRoom = (GroupChatRoom)objChatRoom;
            
                OnSendCommand = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(TextToSend))
                    {
                        Message message = new Message( UserSetting.UserEmail,UserSetting.UserName,TextToSend) ;
                        Messages.Add(message);
                        TextToSend = string.Empty;
                        DependencyService.Get<IFirebaseDatabase>().SetMessage(ChatRoom, message);
                        this.OnPropertyChanged("Messages");
                    }

                });
        }


        public async Task OnAppearing() {
           var TempMessages =  await DependencyService.Get<IFirebaseDatabase>().GetMessage(ChatRoom.RoomID);
            if (TempMessages != null)
            {
                Messages.Clear();
                foreach (Message tempMessage in TempMessages) {
                    var tempStatus = tempMessage.IsRead[UserSetting.UserEmail.Replace(".", ":")];
                    if (tempStatus) {
                        tempMessage.Text = "--This message was destructed--";
                    }
                    Messages.Add(tempMessage);
                }
                this.OnPropertyChanged("Messages");
            }
        }

        public async Task OnDisappearing() {

            List<string> documentsID = new List<string>();
            foreach (Message tempMessage in Messages) {
                if (tempMessage.IsDestruct) 
                    documentsID.Add(tempMessage.MessageId);
            }

           var checkStatus =  await DependencyService.Get<IFirebaseDatabase>().DestructMessage(documentsID);
            if (!checkStatus) {

                await App.Current.MainPage.DisplayAlert("Something happen....", "Message unable to destruct", "Ok");
            }
        }

        public async Task UpdateRoomDestructStatus(bool destructStatus) {
            var UpdateStatus =  await DependencyService.Get<IFirebaseDatabase>().SetRoomDestruct(ChatRoom.RoomID, destructStatus);
            if (UpdateStatus != null) {
                await App.Current.MainPage.DisplayAlert("Something happen....", UpdateStatus, "Ok");
            }
        }

        public void UpdateMessageToRead() {

            DependencyService.Get<IFirebaseDatabase>().SetRead(ChatRoom.RoomID);
        }

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }
    }
}
