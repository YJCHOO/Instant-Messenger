using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pal.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewAttachment : ContentPage
    {
        public WebViewAttachment(string uri)
        {
           //try
            //{
                InitializeComponent();
                var validUri = uri;
                if (uri.StartsWith("uri", StringComparison.CurrentCultureIgnoreCase))
                {
                    validUri = uri.Remove(0, 4);
                }
                else if (uri.Contains("pdf"))
                {

                    Device.OpenUri(new Uri(uri));
                }
                AttachementView.Source = validUri;
            //}catch()
        }


    }
}