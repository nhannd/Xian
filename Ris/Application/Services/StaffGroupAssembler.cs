using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    class StaffGroupAssembler
    {
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
            return new StaffGroupDetail(
                staffGroup.GetRef(),
                staffGroup.Name,
                staffGroup.Description,
				staffGroup.Elective,
                CollectionUtils.Map<Staff, StaffSummary>(staffGroup.Members,
                                                         delegate (Staff staff)
                                                         {
                                                             return staffAssembler.CreateStaffSummary(staff, context);
                                                         }),
				staffGroup.Deactivated
                );
        }

        public void UpdateStaffGroup(StaffGroup group, StaffGroupDetail detail, IPersistenceContext context)
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
        }
    }
}