using System;
using System.Data;
using System.ServiceModel;
using System.Web;
using System.Collections;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    //// <summary>
    /// Summary description for FilesystemInfoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FilesystemInfoService : System.Web.Services.WebService
    {

        [WebMethod]
        public FilesystemServiceProxy.FilesystemInfo GetFilesystemInfo(string path)
        {

            FilesystemServiceProxy.FilesystemInfo fsInfo = null;
            FilesystemServiceProxy.FilesystemServiceClient client = new FilesystemServiceProxy.FilesystemServiceClient();
            try
            {
                fsInfo = client.GetFilesystemInfo(path);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e.StackTrace);
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                    client.Close();
            }
            return fsInfo;
        }
    }
}
