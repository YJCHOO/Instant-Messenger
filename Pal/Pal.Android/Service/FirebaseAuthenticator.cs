using System.Threading.Tasks;
using Pal.Service;
using Firebase.Auth;
using Xamarin.Forms;
using System;

[assembly: Dependency(typeof(Pal.Droid.Service.FirebaseAuthenticator))]
namespace Pal.Droid.Service
{
    class FirebaseAuthenticator : IFirebaseAuthenticator
    {

        private string ExceptionError { get; set; }

        public async Task<string> LoginWithEmailPass(string email, string pass)
        {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, pass);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token; 

        }

        public async Task<string> RegisterWithEmailPassword(string email, string pass)
        {
                    var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, pass);
                    var token = await user.User.GetTokenAsync(false);
                    return token.Token;
        }

    }
}