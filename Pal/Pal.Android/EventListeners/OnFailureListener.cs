using System;
using Android.Gms.Tasks;
using Java.Lang;

namespace Pal.Droid.EventListeners
{
    class OnFailureListener : Java.Lang.Object,IOnFailureListener
    {
        private readonly Action<Java.Lang.Exception> _FailureAction;

        public OnFailureListener(Action<Java.Lang.Exception> FailureAction)
        {
            _FailureAction = FailureAction;

        }

        public void OnFailure(Java.Lang.Exception e) => _FailureAction(e);

    }
}