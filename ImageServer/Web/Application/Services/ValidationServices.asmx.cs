#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using System.Net;
using System.Web.Script.Services;
using System.Web.Services;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Application.ValidationServerProxy;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    /// <summary>
    /// Provides data validation services
    /// </summary>
    [WebService(Namespace = "http://www.clearcanvas.ca/ImageServer/Services/ValidationServices.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [GenerateScriptType(typeof (ValidationResult))]
    [ScriptService]
    public class ValidationServices : WebService
    {
        /// <summary>
        /// Validate the existence of the specified path on the network.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateFilesystemPath(string path)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult res = new  ValidationResult();

            try
            {
                ValidationServiceClient service = new ValidationServiceClient();
                service.Open();
                res = service.CheckPath(path);
                service.Close();
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Error, "ValidationService ValidateFilesystemPath failed: {0}", ex.StackTrace);

                res.Success = false;
                res.ErrorCode = -5000;
                res.ErrorText = "Validation Service is not available at this time.";
                
            }
            
            return res;
        }
    }
}
