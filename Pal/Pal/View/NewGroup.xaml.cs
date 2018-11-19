using Pal.Model;
using Pal.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewGroup : ContentPage
	{
        CreateChatRoomPageModelView vm;

		public NewGroup (ObservableCollection<User> InvitedFriends)
		{
			InitializeComponent ();
            vm = new CreateChatRoomPageModelView();
            this.BindingContext = vm;
            vm.InvitedFriends = InvitedFriends;
		}

        protected override void OnAppearing() {
            vm.InitialNewGroup();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await vm.CreateNewGroup();
        }
    }
}