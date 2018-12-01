using Pal.Model;
using Pal.Service;
using Pal.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class PollViewModel : INotifyPropertyChanged
    {
        public ICommand OnCheckResultCommand { get; set; }
        public ICommand OnCreatePollCommand { get; set; }
        public ICommand OnAddOptionCommand { get; set; }
        public ICommand OnPostCommand { get; set; }
        public ICommand OnCheckResult { get; set; }
        public ICommand OnCloseCommand { get; set; }
        public string InputOption { get; set; }
        public string InputOptionDelete {
            get { return InputOptionDelete; }
            set {
                TempOption.Remove(value);
                OnPropertyChanged("TempOption");
            }
        }
        public string StrTitle { get; set; }
        public ObservableCollection<string> TempOption { get; set; } = new ObservableCollection<string>();
        public Poll _Poll { get; set; } = new Poll();
        public ObservableCollection<SelectableData<string>> Option { get; set; } = new ObservableCollection<SelectableData<string>>();
        private GroupChatRoom groupChatRoom = new GroupChatRoom();
        public bool IsAdmin { get; set; } = false;
        public bool IsClosed { get; set; } = false;
        public bool IsEnablePoll { get; set; } = true;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public PollViewModel(GroupChatRoom groupChatRoom) {
            this.groupChatRoom = groupChatRoom;
            if (groupChatRoom.Admin.Equals(UserSetting.UserEmail)) {
                IsAdmin = true;
                OnPropertyChanged("IsAdmin");
            }

            OnCreatePollCommand = new Command(async() => {
                Option  = new ObservableCollection<SelectableData<string>>();
                if (!_Poll.IsClose)
                {
                    await App.Current.MainPage.DisplayAlert("Something wrong....", "Please close this poll first. ", "Ok");
                    return;
                }

                await App.Current.MainPage.Navigation.PushAsync( new CreatePollView(this.groupChatRoom));
            });

            OnAddOptionCommand = new Command(() =>
            {
                if (!string.IsNullOrWhiteSpace(InputOption)) {
                    TempOption.Add(InputOption);
                }
                InputOption = string.Empty;
                OnPropertyChanged("TempOption");
                OnPropertyChanged("InputOption");
            });

            OnPostCommand = new Command(async() =>
            {
                if (string.IsNullOrWhiteSpace(StrTitle))
                {
                    await App.Current.MainPage.DisplayAlert("Title is required.", "Please entry title.", "Ok");
                    return;
                }
                else if (TempOption.Count <2)
                {
                    await App.Current.MainPage.DisplayAlert("Option is required.", "At least 2 option is required.", "Ok");
                    return;
                }

                Poll poll = new Poll("",groupChatRoom.RoomID, StrTitle,false,TempOption,null);
                var status =  await DependencyService.Get<IFirebaseDatabase>().AddPoll(poll);
                if (status)
                {

                    await App.Current.MainPage.DisplayAlert("Done", "Poll created", "Ok");
                }
                else {
                    await App.Current.MainPage.DisplayAlert("Something wrong....", "Poll Unable to create, Please try again", "Ok");
                }

            });

            OnCheckResult = new Command(async () => {
                bool status = false;

                foreach (string VotedUser in _Poll.Result.Keys) {
                    if (UserSetting.UserEmail.Equals(VotedUser.Replace(":", "."))) {
                        status = true;
                    }

                }

                if (!status)
                {
                    await App.Current.MainPage.DisplayAlert("Something need to do first.", "Please vote first", "Ok");
                }
                else
                {
                    string FinalResult = "";
                    int Count = 0;
                    string OptionResult = "";
                    foreach (string option in _Poll._Option) {
                        OptionResult = option;
                        foreach (string key in _Poll.Result.Keys) {
                            if (_Poll.Result[key].Equals(option)) {
                                Count++;
                            }
                        }
                        FinalResult += OptionResult + ": " + Count+"\n";
                        Count = 0;
                    }

                    await App.Current.MainPage.DisplayAlert("Result",FinalResult,"Ok");

                }
            });

            OnCloseCommand = new Command(async () => 
            {
                if (_Poll.IsClose) {
                    await App.Current.MainPage.DisplayAlert("Something wrong....","This Poll already closed","Ok");
                    return;
                }

                var status = await DependencyService.Get<IFirebaseDatabase>().UpdatePollCloseStatus(_Poll.PollId);
                if (status)
                {
                    await App.Current.MainPage.DisplayAlert("Done", "The poll had been closed.", "Ok");
                    _Poll.IsClose = true;
                    IsClosed = true;
                    IsEnablePoll = false;
                    OnPropertyChanged("IsEnablePoll");
                    OnPropertyChanged("IsClosed");

                }
                else {
                    await App.Current.MainPage.DisplayAlert("Something wrong....", "Poll unable to close, Please try again.", "Ok");
                }
            });
        }

        public async Task InitialPoll() {

            bool IsVoted = false;
            var TempPoll = await DependencyService.Get<IFirebaseDatabase>().GetLastestPoll(groupChatRoom.RoomID);
            _Poll = TempPoll;

            foreach (string votedUser in _Poll.Result.Keys)
            {
                if (votedUser.Equals(UserSetting.UserEmail.Replace(".", ":")))
                {
                    IsVoted = true;
                }
            }

            foreach (string TempStr in _Poll._Option) {
                if (IsVoted && TempStr.Equals(_Poll.Result[UserSetting.UserEmail.Replace(".", ":")]))
                {
                    Option.Add(new SelectableData<string>(TempStr, true));
                }
                else
                {
                    Option.Add(new SelectableData<string>(TempStr, false));
                }
            }

            if (_Poll.IsClose)
            {
                await App.Current.MainPage.DisplayAlert("Closed", "This poll had been closed", "Ok");
                IsEnablePoll = false;
                IsClosed = true;
                OnPropertyChanged("IsEnablePoll");
                OnPropertyChanged("IsClosed");
            }

        }

        public async Task UpadateVoteResult() {

            var VotedUser = _Poll.Result.Where(x => x.Key == UserSetting.UserEmail.Replace(".", ":")).Count();
            if (VotedUser==1) {
                return;
            }

            string Vote = null;
            foreach (SelectableData<string> selectableData in Option) {
                if (selectableData.Selected) {
                    Vote = selectableData.Data;
                }
            }

            _Poll.Result.Add(UserSetting.UserEmail.Replace(".", ":"), Vote);
            var Check = await DependencyService.Get<IFirebaseDatabase>().UpdateResult(_Poll);
            if (Check)
            {
                await App.Current.MainPage.DisplayAlert("Done", "Thank for your vote.", "Ok");
                IsEnablePoll = false;
                OnPropertyChanged("IsEnablePoll");
            }
            else {
                await App.Current.MainPage.DisplayAlert("Something wrong....", "Vote unsuccessful.Please try again.","Ok");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

    }
}
