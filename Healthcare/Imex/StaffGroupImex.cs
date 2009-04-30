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
    [ImexDataClass("StaffGroup")]
    public class StaffGroupImex : XmlEntityImex<StaffGroup, StaffGroupImex.StaffGroupData>
    {
        [DataContract]
		public class StaffGroupData : ReferenceEntityDataBase
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;

			[DataMember]
			public bool Elective;

			[DataMember]
            public List<StaffMemberData> Members;

        }

        [DataContract]
        public class StaffMemberData
        {
            [DataMember]
            public string Id;
        }

        #region Overrides

        protected override IList<StaffGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
            where.Name.SortAsc(0);

            return context.GetBroker<IStaffGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override StaffGroupData Export(StaffGroup entity, IReadContext context)
        {
            StaffGroupData data = new StaffGroupData();
			data.Deactivated = entity.Deactivated;
			data.Name = entity.Name;
            data.Description = entity.Description;
        	data.Elective = entity.Elective;

            data.Members = CollectionUtils.Map<Staff, StaffMemberData>(
                entity.Members,
                delegate(Staff staff)
                {
                    StaffMemberData s = new StaffMemberData();
                    s.Id = staff.Id;
                    return s;
                });

            return data;
        }

        protected override void Import(StaffGroupData data, IUpdateContext context)
        {
            StaffGroup group = LoadOrCreateGroup(data.Name,context);
        	group.Deactivated = data.Deactivated;
            group.Description = data.Description;
        	group.Elective = data.Elective;

            if (data.Members != null)
            {
                foreach (StaffMemberData s in data.Members)
                {
                    StaffSearchCriteria where = new StaffSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    Staff staff = CollectionUtils.FirstElement(context.GetBroker<IStaffBroker>().Find(where));
                    if (staff != null)
                        group.Members.Add(staff);
                }
            }
        }

        #endregion


        private StaffGroup LoadOrCreateGroup(string name, IPersistenceContext context)
        {
            StaffGroup group;
            try
            {
                StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = context.GetBroker<IStaffGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                group = new StaffGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
