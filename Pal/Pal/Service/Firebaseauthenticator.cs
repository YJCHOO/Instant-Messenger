using System;

using System.Threading.Tasks;

namespace Pal.Service
{
    public interface IFirebaseAuthenticator
    {
        Task<string> LoginWithEmailPass(String Email, String Pass);
        Task<string> RegisterWithEmailPassword(String Email, String Pass);
    }
}
