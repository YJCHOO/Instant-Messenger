using Pal.Model;
using Pal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupChatContents : ContentPage
	{
        ChatContentsViewModel VM;
        bool IsLeave = true;

        public GroupChatContents (GroupChatRoom groupChatRoom)
		{
			InitializeComponent ();
            this.BindingContext = VM = new ChatContentsViewModel(groupChatRoom);
            this.Title = groupChatRoom.RoomTilte;
        }

        protected override async void OnDisappearing()
        {
            if (IsTappedToViewAttachment())
            {
                return;
            }

            if (IsLeave)
            {
                await VM.OnDisappearing();
            }
        }

        protected override async void OnAppearing()
        {
            if (IsTappedToViewAttachment())
            {
                return;
            }

            if (IsLeave)
            {
                await VM.OnAppearing();
            }
            IsLeave = true;
        }

        public bool IsTappedToViewAttachment()
        {

            var currentPage = App.Current.MainPage.Navigation.NavigationStack;
            if (currentPage[currentPage.Count - 1].GetType() == typeof(Pal.View.WebViewAttachment) ||
                currentPage[currentPage.Count - 1].GetType() == typeof(Pal.View.PinBoardView))
            {
                return true;
            }
            else { return false; }

        }

        private async void ScheduleDestruct_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("After Read Self-Destruct ? ", "Cancel", null, "Yes", "No");

            bool Self_destruct = false;

            if (action != null)
            {
                if (action.CompareTo("Yes") == 0)
                {

                    Self_destruct = true;
                }
            }
            VM.ChatRoom.IsDestruct = Self_destruct;
            await VM.UpdateRoomDestructStatus(Self_destruct);
        }

        private async void AddAttachments_Clicked(object sender, EventArgs e)
        {
            IsLeave = false;
            var isPicked = await VM.PickAndShowFile();
            if (isPicked)
            {
                VM.DisplayAttachment();
            }
        }

        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            VM.RemoveAttachment();
        }

        private async void PinBoard_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new PinBoardView(VM.GroupChatRoom));
        }
    }
}