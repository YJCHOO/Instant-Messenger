namespace Pal.Service
{
    public interface IFirebaseDatabase
    {
        void GetUser();
        void SetUser(string email,string name);
        void GetMessage();
        void SetMessage();
    }
}
