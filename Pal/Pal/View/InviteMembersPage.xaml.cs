using Pal.Model;
using Pal.ViewModel;
 
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InviteMembersPage : ContentPage
    {
        CreateChatRoomPageModelView vm = new CreateChatRoomPageModelView();
        public InviteMembersPage(ObservableCollection<User> FriendsList)
        {
            InitializeComponent();
            this.BindingContext = vm;
            foreach (User user in FriendsList)
            {
                vm.InviteList.Add(new SelectableData<User>(user, false));
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string InputEmail = e.NewTextValue;

            if (!string.IsNullOrWhiteSpace(InputEmail))
            {
                FriendsListView.ItemsSource = vm.InviteList.Where(i => i.Data.Email.ToLower().Contains(InputEmail.ToLower()));
            }
            else {
                FriendsListView.ItemsSource = vm.InviteList;
            }
        }
    }
}