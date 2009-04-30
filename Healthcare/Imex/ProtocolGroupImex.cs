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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("ProtocolGroup")]
    public class ProtocolGroupImex : XmlEntityImex<ProtocolGroup, ProtocolGroupImex.ProtocolGroupData>
    {
        [DataContract]
        public class ProtocolGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;

            [DataMember]
            public List<ProtocolCodeData> Codes;

            [DataMember]
            public List<ReadingGroupData> ReadingGroups;
        }

        [DataContract]
        public class ProtocolCodeData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;
        }

        [DataContract]
        public class ReadingGroupData
        {
            [DataMember]
            public string Name;
        }

        #region Overrides

        protected override IList<ProtocolGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ProtocolGroupSearchCriteria where = new ProtocolGroupSearchCriteria();
            where.Name.SortAsc(0);

            return context.GetBroker<IProtocolGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ProtocolGroupData Export(ProtocolGroup group, IReadContext context)
        {
            ProtocolGroupData data = new ProtocolGroupData();
            data.Name = group.Name;
            data.Description = group.Description;

            data.Codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeData>(group.Codes,
                delegate(ProtocolCode code)
                {
                    ProtocolCodeData s = new ProtocolCodeData();
                    s.Name = code.Name;
                    s.Description = code.Description;
                    return s;
                });

            data.ReadingGroups = CollectionUtils.Map<ReadingGroup, ReadingGroupData>(group.ReadingGroups,
                delegate(ReadingGroup rg)
                {
                    ReadingGroupData s = new ReadingGroupData();
                    s.Name = rg.Name;
                    return s;
                });

            return data;
        }

        protected override void Import(ProtocolGroupData data, IUpdateContext context)
        {
            ProtocolGroup group = LoadOrCreateProtocolGroup(data.Name, context);
            group.Description = data.Description;

            if (data.Codes != null)
            {
                foreach (ProtocolCodeData s in data.Codes)
                {
                    ProtocolCode code = LoadOrCreateProtocolCode(s.Name, context);
                    code.Description = s.Description;

                    group.Codes.Add(code);
                }
            }

            if (data.ReadingGroups != null)
            {
                foreach (ReadingGroupData s in data.ReadingGroups)
                {
                    ReadingGroupSearchCriteria criteria = new ReadingGroupSearchCriteria();
                    criteria.Name.EqualTo(s.Name);

                    IReadingGroupBroker broker = context.GetBroker<IReadingGroupBroker>();
                    ReadingGroup rg = CollectionUtils.FirstElement(broker.Find(criteria));
                    if (rg != null)
                        group.ReadingGroups.Add(rg);
                }
            }


        }

        #endregion

        private ProtocolGroup LoadOrCreateProtocolGroup(string name, IPersistenceContext context)
        {
            ProtocolGroup group;
            try
            {
                // see if already exists in db
                ProtocolGroupSearchCriteria where = new ProtocolGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = context.GetBroker<IProtocolGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                group = new ProtocolGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }

            return group;
        }

        private ProtocolCode LoadOrCreateProtocolCode(string name, IPersistenceContext context)
        {
            ProtocolCode code;
            try
            {
                // see if already exists in db
                ProtocolCodeSearchCriteria where = new ProtocolCodeSearchCriteria();
                where.Name.EqualTo(name);
                code = context.GetBroker<IProtocolCodeBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                code = new ProtocolCode();
                code.Name = name;
                context.Lock(code, DirtyState.New);
            }

            return code;
        }
    }
}
