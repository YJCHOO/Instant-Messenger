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
	public partial class AddMoments : ContentPage
	{
        SocialViewModel VM;
		public AddMoments ()
		{
			InitializeComponent ();
            VM = new SocialViewModel();
            BindingContext = VM;
		}
	}
}