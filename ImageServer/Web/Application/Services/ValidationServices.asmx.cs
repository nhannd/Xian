#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

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

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(path))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = "Path cannot be empty";
                return result;
            }

            FilesystemServiceProxy.FilesystemInfo fsInfo = null;
            FilesystemServiceProxy.FilesystemServiceClient client = new FilesystemServiceProxy.FilesystemServiceClient();
            result.Success = false;
            try
            {
                fsInfo = client.GetFilesystemInfo(path);
                if (fsInfo.Exists)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ErrorText = String.Format("{0} is either invalid or not reachable.", path);
                }
                return result;
            }
            catch(Exception e)
            {
                result.Success = false;
                result.ErrorCode = 100;
                result.ErrorText = String.Format("Cannot validate path {0}: {1}", path, e.Message);
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                    client.Close();
            }

            return result;
        }

        /// <summary>
        /// Validate the existence of a user name.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="originalUsername"></param>/// 
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateUsername(string username, string originalUsername)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(username))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = "Username is required.";
                return result;
            }

            UserManagementController controller = new UserManagementController();

            if (controller.ExistsUsername(username) && !username.Equals(originalUsername))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = "Username already exists.";
                return result;
                
            } else
            {
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// Validate the existence of a user group.
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <param name="originalGroupName"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [WebMethod]
        public ValidationResult ValidateUserGroupName(string userGroupName, string originalGroupName)
        {
            // This web service in turns call a WCF service which resides on the same or different systems.

            ValidationResult result = new ValidationResult();
            if (String.IsNullOrEmpty(userGroupName))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = "User Group is required.";
                return result;
            }

            UserManagementController controller = new UserManagementController();

            if (controller.ExistsUsergroup(userGroupName) && !userGroupName.Equals(originalGroupName))
            {
                result.Success = false;
                result.ErrorCode = -1;
                result.ErrorText = "User Group already exists.";
                return result;
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// Validate a ServerRule for proper formatting.
        /// </summary>
        /// <param name="serverRule">A string representing the rule.</param>
        /// <param name="ruleType">An string enumerated value of <see cref="ServerRuleTypeEnum"/></param>
        /// <returns>The result of the validation.</returns>
        [WebMethod]
        public ValidationResult ValidateServerRule(string serverRule, string ruleType)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrEmpty(serverRule))
            {
                result.ErrorText = "Server Rule XML must be specified";
                result.Success = false;
                result.ErrorCode = -5000;
                return result;
            }

        	ServerRuleTypeEnum type;
			try
			{
				type = ServerRuleTypeEnum.GetEnum(ruleType);
			}
			catch (Exception e)
			{
				result.ErrorText = "Unable to parse rule type: " + e.Message;
				result.Success = false;
				result.ErrorCode = -5000;
				return result;
			}

        	XmlDocument theDoc = new XmlDocument();

            try
            {
                string xml = Microsoft.JScript.GlobalObject.unescape(serverRule);
                theDoc.LoadXml(xml);
            }
            catch (Exception e)
            {
                result.ErrorText = "Unable to parse XML: " + e.Message;
                result.Success = false;
                result.ErrorCode = -5000;
                return result;
            }

            string error;
            if (false == ClearCanvas.ImageServer.Rules.Rule.ValidateRule(type, theDoc, out error))
            {
                result.ErrorText = error;
                result.Success = false;
                result.ErrorCode = -5000;
            }
            else
                result.Success = true;

            return result;
        }
    }
}
