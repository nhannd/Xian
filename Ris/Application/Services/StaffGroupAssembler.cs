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

using System.Collections.Generic;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    class StaffGroupAssembler
    {
		private class StaffGroupWorklistCollectionSynchronizeHelper : CollectionSynchronizeHelper<Worklist, WorklistSummary>
		{
			private readonly StaffGroup _group;
			private readonly IPersistenceContext _context;

			public StaffGroupWorklistCollectionSynchronizeHelper(StaffGroup group, IPersistenceContext context)
				: base(false, true)
			{
				_group = group;
				_context = context;
			}

			protected override bool CompareItems(Worklist destItem, WorklistSummary sourceItem)
			{
				return sourceItem.WorklistRef.Equals(destItem.GetRef(), true);
			}

			protected override void AddItem(WorklistSummary sourceItem, ICollection<Worklist> dest)
			{
				Worklist worklist = _context.Load<Worklist>(sourceItem.WorklistRef, EntityLoadFlags.Proxy);
				worklist.GroupSubscribers.Add(_group);
				dest.Add(worklist);
			}

			protected override void RemoveItem(Worklist destItem, ICollection<Worklist> dest)
			{
				destItem.GroupSubscribers.Remove(_group);
				dest.Remove(destItem);
			}
		}

        public StaffGroupSummary CreateSummary(StaffGroup staffGroup)
        {
            return new StaffGroupSummary(
                staffGroup.GetRef(),
                staffGroup.Name,
                staffGroup.Description,
				staffGroup.Elective,
				staffGroup.Deactivated);
        }

        public StaffGroupDetail CreateDetail(StaffGroup staffGroup, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
			WorklistAssembler worklistAssembler = new WorklistAssembler();

        	IList<Worklist> worklists = context.GetBroker<IWorklistBroker>().Find(staffGroup);

        	return new StaffGroupDetail(
        		staffGroup.GetRef(),
        		staffGroup.Name,
        		staffGroup.Description,
        		staffGroup.Elective,
        		CollectionUtils.Map<Staff, StaffSummary>(staffGroup.Members,
        		                                         delegate(Staff staff)
        		                                         	{
        		                                         		return staffAssembler.CreateStaffSummary(staff, context);
        		                                         	}),
        		CollectionUtils.Map<Worklist, WorklistSummary>(worklists,
        		                                         delegate(Worklist worklist)
    		                                               	{
    		                                               		return worklistAssembler.GetWorklistSummary(worklist, context);
    		                                               	}),
				staffGroup.Deactivated
                );
        }

        public void UpdateStaffGroup(StaffGroup group, StaffGroupDetail detail, bool updateWorklist, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;
        	group.Elective = detail.IsElective;
        	group.Deactivated = detail.Deactivated;

            group.Members.Clear();
            CollectionUtils.ForEach(detail.Members,
                 delegate(StaffSummary summary)
                 {
                     group.AddMember(context.Load<Staff>(summary.StaffRef, EntityLoadFlags.Proxy));
                 });

			if (updateWorklist)
			{
				StaffGroupWorklistCollectionSynchronizeHelper helper = new StaffGroupWorklistCollectionSynchronizeHelper(group, context);
				IList<Worklist> originalWorklists = context.GetBroker<IWorklistBroker>().Find(group);
				helper.Synchronize(originalWorklists, detail.Worklists);
			}
        }
    }
}