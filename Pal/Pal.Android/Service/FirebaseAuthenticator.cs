using System.Threading.Tasks;
using Pal.Service;
using Firebase.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseAuthenticator))]
namespace Pal.Droid.Service
{
    class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public async Task<string> LoginWithEmailPass(string email, string pass)
        {    
            var user = await FirebaseAuth.GetInstance(MainActivity.app).SignInWithEmailAndPasswordAsync(email, pass);
            return user.User.Email;
        }

        public async Task<string> RegisterWithEmailPassword(string email, string pass)
        {
            var user = await FirebaseAuth.GetInstance(MainActivity.app).CreateUserWithEmailAndPasswordAsync(email, pass);
            return user.User.Email;
        }
    }
}