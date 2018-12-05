using Android.Hardware.Camera2;
using System;

namespace Pal.Droid.Listener
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        Action<CameraCaptureSession> _EventAction;

        public CameraCaptureSessionCallback(Action<CameraCaptureSession> eventAction) {
            _EventAction = eventAction;

        }

        public override void OnConfigured(CameraCaptureSession session) => _EventAction(session);

        public override void OnConfigureFailed(CameraCaptureSession session) => _EventAction(session);
    }
}