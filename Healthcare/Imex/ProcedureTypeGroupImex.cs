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

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("ProcedureTypeGroup")]
    public class ProcedureTypeGroupImex : XmlEntityImex<ProcedureTypeGroup, ProcedureTypeGroupImex.ProcedureTypeGroupData>
    {
        [DataContract]
        public class ProcedureTypeGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Class;

            [DataMember]
            public string Description;

            [DataMember]
            public List<ProcedureTypeData> ProcedureTypes;

        }

        [DataContract]
        public class ProcedureTypeData
        {
            [DataMember]
            public string Id;
        }

        #region Overrides

        protected override IList<ProcedureTypeGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ProcedureTypeGroupSearchCriteria where = new ProcedureTypeGroupSearchCriteria();
            where.Name.SortAsc(0);
            return context.GetBroker<IProcedureTypeGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ProcedureTypeGroupData Export(ProcedureTypeGroup entity, IReadContext context)
        {
            ProcedureTypeGroupData data = new ProcedureTypeGroupData();
            data.Name = entity.Name;
            data.Class = entity.GetClass().FullName;
            data.Description = entity.Description;

            data.ProcedureTypes = CollectionUtils.Map<ProcedureType, ProcedureTypeData>(
                entity.ProcedureTypes,
                delegate(ProcedureType pt)
                {
                    ProcedureTypeData ptdata = new ProcedureTypeData();
                    ptdata.Id = pt.Id;
                    return ptdata;
                });

            return data;
        }

        protected override void Import(ProcedureTypeGroupData data, IUpdateContext context)
        {
            ProcedureTypeGroup group = LoadOrCreateProcedureTypeGroup(data.Name, data.Class, context);
            group.Description = data.Description;

            if (data.ProcedureTypes != null)
            {
                foreach (ProcedureTypeData s in data.ProcedureTypes)
                {
                    ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    ProcedureType pt = CollectionUtils.FirstElement(context.GetBroker<IProcedureTypeBroker>().Find(where));
                    if (pt != null)
                        group.ProcedureTypes.Add(pt);
                }
            }
        }

        #endregion


        private ProcedureTypeGroup LoadOrCreateProcedureTypeGroup(string name, string className, IPersistenceContext context)
        {
            Type groupClass = ProcedureTypeGroup.GetSubClass(className, context);
            ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
            criteria.Name.EqualTo(name);

            ProcedureTypeGroup group = null;
            try
            {
                group = context.GetBroker<IProcedureTypeGroupBroker>().FindOne(criteria, groupClass);
            }
            catch (EntityNotFoundException)
            {
                group = (ProcedureTypeGroup)Activator.CreateInstance(groupClass);
                group.Name = name;

                context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
