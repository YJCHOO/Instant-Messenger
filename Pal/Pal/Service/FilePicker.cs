using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;

namespace Pal.Service
{
    public class FilePicker
    {
        private static FilePicker Instance = new FilePicker();

        private FilePicker() { }

        public static FilePicker GetInstance() {
            return Instance;
        }
        public async Task<bool> CheckPermission() {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await App.Current.MainPage.DisplayAlert("Permission Request", "Storage permission is needed for file picking", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);

                    if (results.ContainsKey(Permission.Storage))
                    {
                        status = results[Permission.Storage];
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await App.Current.MainPage.DisplayAlert("Permission request", "Storage permission was denied.", "OK");
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<FileData> PickAndShowFile() {
            var pickedFile = await CrossFilePicker.Current.PickFile();
            if (pickedFile != null)
            {
                return pickedFile;
            }
            else
            {
                return null;
            }
        }

        public string IsSupportedType(string FileType)
        {
            string Type = null;

            if (FileType.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                   FileType.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                Type = "Img";
            else if (FileType.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
                Type = "Video";
            else if (FileType.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                Type = "PDF";
            return Type;
        }

        public string IsSupportedMomentType(string FileType) {

            string Type = null;
            if (FileType.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                   FileType.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                Type = "Img";
            else if (FileType.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
                Type = "Video";

            return Type;
        }
    }
}
