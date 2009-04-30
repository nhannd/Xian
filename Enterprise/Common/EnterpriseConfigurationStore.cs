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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Enterprise implementation of <see cref="IConfigurationStore"/>.
    /// </summary>
    [ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
    public class EnterpriseConfigurationStore : IConfigurationStore
    {
        #region IConfigurationStore Members

        /// <summary>
        /// Obtains the specified document for the specified user and instance key.  If user is null,
        /// the shared document is obtained.
        /// </summary>
        public TextReader GetDocument(string name, Version version, string user, string instanceKey)
        {
            string content = null;
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    content = service.GetConfigurationDocument(
						new GetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(name, version, user, instanceKey))).Content;
                });

            if(content == null)
                throw new ConfigurationDocumentNotFoundException(name, version, user, instanceKey);

            return new StringReader(content);
        }

        /// <summary>
        /// Stores the specified document for the current user and instance key.  If user is null,
        /// the document is stored as a shared document.
        /// </summary>
        public void PutDocument(string name, Version version, string user, string instanceKey, TextReader content)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.SetConfigurationDocument(
						new SetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(name, version, user, instanceKey), content.ReadToEnd()));
                });
        }

        #endregion
    }
}
