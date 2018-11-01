

using Android.Gms.Tasks;
using System;

namespace Pal.Droid.EventListeners
{
    class OnCompleteEventHandleListener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Task> _completeAction;

        public OnCompleteEventHandleListener(Action<Task> completeAction) {
            _completeAction = completeAction;

        }

        public void OnComplete(Task task) => _completeAction(task);
    }
}