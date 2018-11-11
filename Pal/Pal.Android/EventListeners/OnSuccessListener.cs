using Android.Gms.Tasks;
using System;

namespace Pal.Droid.EventListeners
{
    class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private readonly Action<Java.Lang.Object> _SuccessAction;

        public OnSuccessListener(Action<Java.Lang.Object> SuccessAction)
        {
            _SuccessAction = SuccessAction;
        }

        public void OnSuccess(Java.Lang.Object result) => _SuccessAction(result);
    }
}