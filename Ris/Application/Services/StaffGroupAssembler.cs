#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class StaffGroupAssembler
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

        public void UpdateStaffGroup(StaffGroup group, StaffGroupDetail detail, bool updateWorklist, bool isNewStaffGroup, IPersistenceContext context)
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
				IList<Worklist> originalWorklists = isNewStaffGroup 
					? new List<Worklist>()
					: context.GetBroker<IWorklistBroker>().Find(group);

				helper.Synchronize(originalWorklists, detail.Worklists);
			}
        }
    }
}