using Pal.Model;
using Pal.Service;
using Pal.View.CustomViewCell;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Pal.ViewModel
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        readonly DataTemplate IncomingDataTemplate;
        readonly DataTemplate OutgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.IncomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.OutgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));

        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as Message;
            if (messageVm == null)
                return null;

            return (string.Compare(messageVm.SenderEmail,UserSetting.UserEmail)==0) ? OutgoingDataTemplate : IncomingDataTemplate;

        }

    }
}
