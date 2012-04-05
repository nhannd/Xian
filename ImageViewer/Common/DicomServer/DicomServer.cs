using System;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public abstract class DicomServer : IDicomServer
    {
        static DicomServer()
        {
            try
            {

                var service = Platform.GetService<IDicomServer>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch(EndpointNotFoundException)
            {
                //This doesn't mean it's not supported, it means it's not running.
                IsSupported = true;
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Local DICOM Server is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Local DICOM Server is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        public static bool IsRunning
        {
            get { return new DicomServerBridge().IsRunning; }
        }

        #region IDicomServer Members

        public abstract GetListenerStateResult GetListenerState(GetListenerStateRequest request);
        public abstract RestartListenerResult RestartListener(RestartListenerRequest request);

        #endregion
    }
}
