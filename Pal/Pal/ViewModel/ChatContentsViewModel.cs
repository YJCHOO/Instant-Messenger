using Pal.Model;
using Pal.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;


namespace Pal.ViewModel
{
    public class ChatContentsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public string TextToSend { get; set; }
        public ICommand OnSendCommand { get; set; }
        

        public ChatContentsViewModel()
        {

                this.Messages.Add(new Message() { Text = "Hi" });
                this.Messages.Add(new Message() { Text = "How are you?" });
                this.Messages.Add(new Message() { Text="Hi",User=App.User});

                OnSendCommand = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(TextToSend))
                    {
                        this.Messages.Add(new Message() { Text = TextToSend, User = App.User });
                        TextToSend = string.Empty;
                        
                        this.OnPropertyChanged("Messages");
                    }

                });
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }
    }
}
