
using System;
using System.Threading.Tasks;
using Firebase.Storage;
using Pal.Droid.EventListeners;
using Pal.Model;
using Pal.Service;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseStorageService))]

namespace Pal.Droid.Service
{
    public class FirebaseStorageService : IFirebaseStorage
    {
        FirebaseStorage storage = FirebaseStorage.GetInstance("gs://palproject-127b0.appspot.com");

        public Task<Attachment> UploadFile(FileData fileData)
        {
            TaskCompletionSource<Attachment> ResultCompletionSource = new TaskCompletionSource<Attachment>();
            string fileExtension = GetExtension(fileData.FileName);
            string fileNameStr = fileData.FileName;

            string path = "userFile/"+Guid.NewGuid().ToString()+ fileExtension;

            try
            {
                StorageReference storageReference = storage.GetReference(path);
                StorageMetadata storageMetadata = new StorageMetadata.Builder()
                    .SetCustomMetadata("FileName", fileNameStr)
                    .Build();

                UploadTask uploadTask= storageReference.PutStream(fileData.GetStream(), storageMetadata);
                uploadTask.AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task uploadFile) =>
                {
                    if (uploadFile.IsSuccessful) {
                        var TaskResult = uploadFile.Result;
                        var uri = ((UploadTask.TaskSnapshot)TaskResult).DownloadUrl.ToString();
                        Attachment attachment = new Attachment(fileNameStr, uri);
                        ResultCompletionSource.SetResult(attachment);
                    }
                }));
            }
            catch (Exception e) {
                ResultCompletionSource.TrySetException(e);
            }
            return ResultCompletionSource.Task;
        }

        public Task<string> UploadMoments(FileData fileData)
        {
            TaskCompletionSource<string> ResultCompletionSource = new TaskCompletionSource<string>();
            string fileExtension = GetMomentExtension(fileData.FileName);
            string fileNameStr = fileData.FileName;

            string path = "moments/" + Guid.NewGuid().ToString() + fileExtension;

            try
            {
                StorageReference storageReference = storage.GetReference(path);
                UploadTask uploadTask = storageReference.PutStream(fileData.GetStream());
                uploadTask.AddOnCompleteListener(new OnCompleteEventHandleListener((Android.Gms.Tasks.Task uploadFile) =>
                {
                    if (uploadFile.IsSuccessful)
                    {
                        var TaskResult = uploadFile.Result;
                        var uri = ((UploadTask.TaskSnapshot)TaskResult).DownloadUrl.ToString();
                        
                        ResultCompletionSource.SetResult(uri);
                    }
                }));
            }
            catch (Exception e)
            {
                ResultCompletionSource.TrySetException(e);
            }
            return ResultCompletionSource.Task;
        }

        private string GetExtension(string FileName) {

            if (FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase))
                return ".jpg";
            else if (FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                return ".png";
            else if (FileName.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
                return  ".mp4";
            else 
                return ".pdf";
        }

        private string GetMomentExtension(string FileName)
        {
            if (FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase))
                return ".jpg";
            else if (FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                return ".png";
            else
                return ".mp4";
        }



    }
}