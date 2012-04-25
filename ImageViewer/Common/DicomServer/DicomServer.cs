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
            catch (UnknownServiceException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Local DICOM Server is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Local DICOM Server is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        public static bool IsListening
        {
            get
            {
                {
                    if (!IsSupported)
                        return false;

                    try
                    {
                        bool state = false;
                        Platform.GetService<IDicomServer>(s =>
                            state = s.GetListenerState(new GetListenerStateRequest()).State == ServiceStateEnum.Started);
                        return state;
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Debug, e, "The local DICOM Server could not be contacted.");
                        return false;
                    }
                }
            }
        }

        public static string AETitle
        {
            get { return GetConfiguration().AETitle; }
        }

        public static string HostName
        {
            get { return GetConfiguration().HostName; }
        }

        public static int Port
        {
            get { return GetConfiguration().Port; }
        }

        public static DicomServerConfiguration GetConfiguration()
        {
            DicomServerConfiguration configuration = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => configuration = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration);
            return configuration;
        }

        public static void UpdateConfiguration(string hostName, string aeTitle, int port)
        {
            Platform.GetService<IDicomServerConfiguration>(
                s => s.UpdateConfiguration(new UpdateDicomServerConfigurationRequest
                                               {
                                                    Configuration = new DicomServerConfiguration
                                                                     {
                                                                         AETitle = aeTitle, HostName = hostName, Port = port
                                                                     }  
                                               }));
        }

        public static void RestartListener()
        {
            Platform.GetService<IDicomServer>(s => s.RestartListener(new RestartListenerRequest()));
        }

        #region IDicomServer Members

        public abstract GetListenerStateResult GetListenerState(GetListenerStateRequest request);
        public abstract RestartListenerResult RestartListener(RestartListenerRequest request);

        #endregion
    }
}
