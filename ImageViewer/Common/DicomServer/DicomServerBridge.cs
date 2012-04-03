using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public class DicomServerBridge
    {
        public bool IsRunning
        {
            get
            {
                if (!DicomServer.IsSupported)
                    return false;

                try
                {
                    bool state = false;
                    Platform.GetService<IDicomServer>(s => state = s.GetListenerState(new GetListenerStateRequest()).State == ServiceState.Started);
                    return state;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Debug, e, "The local dicom server could not be contacted.");
                    return false;
                }
            }
        }

        public void RestartListener()
        {
            Platform.GetService<IDicomServer>(s => s.RestartListener(new RestartListenerRequest()));
        }
    }
}
