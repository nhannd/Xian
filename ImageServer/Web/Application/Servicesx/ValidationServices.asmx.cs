using System;
using System.Data;
using System.IO;
using System.Web;
using System.Collections;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    /// <summary>
    /// Summary description for ValidationServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [GenerateScriptType(typeof(ValidationResult))]
    [ScriptService]
    public class ValidationServices : System.Web.Services.WebService
    {

        [WebMethod]
        public bool ValidateFilesystemPath(string path)
        {
            ValidationResult res = new ValidationResult();
            if (Directory.Exists(path))
            {
                res.Success = true;
            }
            else
            {
                res.Success = false;
                res.ErrorCode = -1;
                res.ErrorText = "Path " + path + " doesn't exist";
            }

            return res.Success;
        }
    }
}
