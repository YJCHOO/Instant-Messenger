using Pal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace Pal.ViewModel
{
    class CreateChatRoomPageModelView : INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; } 
        public string Search {
            get { return this.Search; }
            set {
                if (this.Search != value) {
                    this.Search = value;
                    this.OnPropertyChanged(this.Search);

                }
            }


        }
        public ICommand OncreateRoomCommand { get; set; }


        public CreateChatRoomPageModelView() {
            

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }
    }
}
