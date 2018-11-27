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
        readonly DataTemplate IncomingDataTemplate,
                              IncomingImgAttachmentViewCell,
                              IncomingVideoAttachementViewCell,
                              IncomingDocAttachmentViewCell;



        readonly DataTemplate OutgoingDataTemplate,
                              OutgoingImgViewCell,
                              OutgoingVideoAttachmentViewCell, 
                              OutgoingDocAttachmentViewCell;

        public ChatTemplateSelector()
        {
            //Incoming
            this.IncomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.IncomingImgAttachmentViewCell = new DataTemplate(typeof(IncomingImgAttachmentViewCell));
            this.IncomingVideoAttachementViewCell = new DataTemplate(typeof(IncomingVideoAttachementViewCell));
            this.IncomingDocAttachmentViewCell = new DataTemplate(typeof(IncomingDocAttachmentViewCell));

            //Outgoing
            this.OutgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.OutgoingImgViewCell = new DataTemplate(typeof(OutgoingImgViewCell));
            this.OutgoingVideoAttachmentViewCell = new DataTemplate(typeof(OutgoingVideoAttachmentViewCell));
            this.OutgoingDocAttachmentViewCell = new DataTemplate(typeof(OutgoingDocAttachmentViewCell));


        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as Message;
            if (messageVm == null)
                return null;

            if (string.Compare(messageVm.SenderEmail, UserSetting.UserEmail) == 0) 
            {
                //outgoing 
                if (string.IsNullOrEmpty(messageVm.AttachmentUri))
                {

                    return OutgoingDataTemplate;
                }
                else if (messageVm.AttachmentFileName.EndsWith("jpg", StringComparison.CurrentCultureIgnoreCase) || messageVm.AttachmentFileName.EndsWith("png", StringComparison.CurrentCultureIgnoreCase))
                {

                    return OutgoingImgViewCell;
                }
                else if (messageVm.AttachmentFileName.EndsWith("mp4", StringComparison.CurrentCultureIgnoreCase))
                {

                    return OutgoingVideoAttachmentViewCell;
                }
                else {

                    return OutgoingDocAttachmentViewCell;
                }



            }
            else {

                //incoming
                if (string.IsNullOrEmpty(messageVm.AttachmentUri))
                {
                    return IncomingDataTemplate;
                }
                else if (messageVm.AttachmentFileName.EndsWith("jpg", StringComparison.CurrentCultureIgnoreCase) || messageVm.AttachmentFileName.EndsWith("png", StringComparison.CurrentCultureIgnoreCase))
                {

                    return IncomingImgAttachmentViewCell;
                }
                else if (messageVm.AttachmentFileName.EndsWith("mp4", StringComparison.CurrentCultureIgnoreCase))
                {

                    return IncomingVideoAttachementViewCell;
                }
                else
                {
                    return IncomingDocAttachmentViewCell;
                }
            }
        }
    }
}
