using System.Threading.Tasks;

namespace Pal.Service
{
    public interface IFirebaseDatabase
    {
        void GetUser(string email);
        void SetUser(string email,string name);
        void UpdateUser(string TempEmail, string username);
        void GetMessage();
        void SetMessage();
    }
}
