using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Pal.Droid.Listener
{
    class CameraStateListener : CameraDevice.StateCallback
    {

        Action<CameraDevice> _CameraState;
        Action<CameraDevice, CameraError> _CameraStateWithError;

        public CameraStateListener(Action<CameraDevice> CameraState) {
            _CameraState = CameraState;

        }

        public CameraStateListener(Action<CameraDevice,CameraError> CameraStateWithError) {
            _CameraStateWithError = CameraStateWithError;
        }

        public override void OnDisconnected(CameraDevice camera) => _CameraState(camera);

        public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error) => _CameraStateWithError(camera, error);

        public override void OnOpened(CameraDevice camera) => _CameraState(camera);
    }
}