using System;
using System.IO;
using System.ServiceModel;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Web.Services.Shreds.Management
{

    // The following settings must be added to your configuration file in order for 
    // the new WCF service item added to your project to work correctly.

    // <system.serviceModel>
    //    <services>
    //      <!-- Before deployment, you should remove the returnFaults behavior configuration to avoid disclosing information in exception messages -->
    //      <service type="ClearCanvas.ImageServer.Web.Services.Shreds.ManagementServer.FilesystemService" behaviorConfiguration="returnFaults">
    //        <endpoint contract="ClearCanvas.ImageServer.Web.Services.Shreds.MiscellaneousServer.IFilesystemService" binding="wsHttpBinding"/>
    //      </service>
    //    </services>
    //    <behaviors>
    //      <serviceBehaviors>
    //        <behavior name="returnFaults" >
    //          <serviceDebug includeExceptionDetailInFaults="true" />
    //        </behavior>
    //       </serviceBehaviors>
    //    </behaviors>
    // </system.serviceModel>


    // A WCF service consists of a contract (defined below), 
    // a class which implements that interface, and configuration 
    // entries that specify behaviors and endpoints associated with 
    // that implementation (see <system.serviceModel> in your application
    // configuration file).
    

    [ServiceContract()]
    public interface IFilesystemService
    {
        [OperationContract]
        FilesystemInfo GetFilesystemInfo(string path);
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class FilesystemService : WcfShred, IFilesystemService
    {

        #region IFilesystemService Members

        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return FilesystemUtils.GetDirectoryInfo(path);
        }

        #endregion

        public override void Start()
        {
            try
            {
                ServiceEndpointDescription sed = StartHttpHost<FilesystemService, IFilesystemService>("FilesystemService", SR.FilesystemQueryServiceDisplayDescription);

            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, "Failed to start {0} : {1}", SR.FilesystemQueryServiceDisplayName, e.StackTrace);

            }
        }

        public override void Stop()
        {
            StopHost("FilesystemService");
        }

        public override string GetDisplayName()
        {
            return SR.FilesystemQueryServiceDisplayName;
        }

        public override string GetDescription()
        {
            return SR.FilesystemQueryServiceDisplayDescription;
        }
    }

}

