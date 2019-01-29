using System;
using Firebase.Firestore;

namespace Pal.Droid.EventListeners
{
   public class EventListener : Java.Lang.Object, IEventListener
    {
        private readonly Action<Java.Lang.Object,FirebaseFirestoreException> _eventAction;

        public EventListener(Action<Java.Lang.Object,FirebaseFirestoreException> eventAction)
        {
            _eventAction = eventAction;
        }

        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException exception) => _eventAction(obj, exception);
    }
}