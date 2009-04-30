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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Configuration.Brokers;

namespace ClearCanvas.Enterprise.Configuration.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Configuration", ItemsPerFile = 1)]
    public class ConfigurationImex : XmlEntityImex<ConfigurationDocument, ConfigurationImex.ConfigurationData>
    {
        [DataContract]
        public class ConfigurationData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Version;

            [DataMember]
            public string User;

            [DataMember]
            public string InstanceKey;
            
            [DataMember]
            public string Body;
        }


        #region Overrides

        protected override IList<ConfigurationDocument> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ConfigurationDocumentSearchCriteria where = new ConfigurationDocumentSearchCriteria();
            where.DocumentName.SortAsc(0);
            where.DocumentVersionString.SortAsc(1);
            where.User.SortAsc(2);
            where.InstanceKey.SortAsc(3);

            return context.GetBroker<IConfigurationDocumentBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ConfigurationData Export(ConfigurationDocument configDocument, IReadContext context)
        {
            ConfigurationData data = new ConfigurationData();
            data.Name = configDocument.DocumentName;
            data.Version = configDocument.DocumentVersionString;
            data.User = configDocument.User;
            data.InstanceKey = configDocument.InstanceKey;
            data.Body = configDocument.Body.DocumentText;

            return data;
        }

        protected override void Import(ConfigurationData data, IUpdateContext context)
        {
            ConfigurationDocument document = LoadOrCreateDocument(data.Name, data.Version, data.User, data.InstanceKey, context);
            document.Body.DocumentText = data.Body;
        }

        #endregion


        private ConfigurationDocument LoadOrCreateDocument(string name, string version, string user, string instanceKey, IPersistenceContext context)
        {
            ConfigurationDocument document = null;

            try
            {
                ConfigurationDocumentSearchCriteria criteria = new ConfigurationDocumentSearchCriteria();
                criteria.DocumentName.EqualTo(name);
                criteria.DocumentVersionString.EqualTo(version);

                if (!string.IsNullOrEmpty(instanceKey))
                    criteria.InstanceKey.EqualTo(instanceKey);
                else
                    criteria.InstanceKey.IsNull();

                if (!string.IsNullOrEmpty(user))
                    criteria.User.EqualTo(user);
                else
                    criteria.User.IsNull();

                IConfigurationDocumentBroker broker = context.GetBroker<IConfigurationDocumentBroker>();
                document = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                document = new ConfigurationDocument(
                    name,
                    version,
                    StringUtilities.NullIfEmpty(user),
                    StringUtilities.NullIfEmpty(instanceKey));
                context.Lock(document, DirtyState.New);
            }
            return document;
        }
    }
}

