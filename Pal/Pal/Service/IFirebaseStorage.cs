using Pal.Model;
using Plugin.FilePicker.Abstractions;

using System.Threading.Tasks;

namespace Pal.Service
{
    public interface IFirebaseStorage
    {
        Task<Attachment> UploadFile(FileData fileData);
        Task<string> UploadMoments(FileData fileData);
    }
}
