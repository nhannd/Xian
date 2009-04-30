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
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
    public class ConfigurationServiceRecorder : ServiceOperationRecorderBase
    {
		/// <summary>
		/// Gets the category that should be assigned to the audit entries.
		/// </summary>
		public override string Category
        {
            get { return "Configuration"; }
        }

		/// <summary>
		/// Writes the detailed message to the specified XML writer.
		/// </summary>
		protected override bool WriteXml(XmlWriter writer, ServiceOperationInvocationInfo info)
        {
            // don't bother logging failed attempts
            if(info.Exception != null)
                return false;

			ConfigurationDocumentRequestBase request = (ConfigurationDocumentRequestBase)info.Arguments[0];

            writer.WriteStartDocument();
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", info.OperationMethodInfo.Name);
            writer.WriteAttributeString("documentName", request.DocumentKey.DocumentName);
            writer.WriteAttributeString("documentVersion", request.DocumentKey.Version.ToString());
            writer.WriteAttributeString("documentUser", request.DocumentKey.User ?? "{application}");
            writer.WriteAttributeString("instanceKey", request.DocumentKey.InstanceKey ?? "{default}");
            writer.WriteEndElement();
            writer.WriteEndDocument();

            return true;
        }
    }
}
