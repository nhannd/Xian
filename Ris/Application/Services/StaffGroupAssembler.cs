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
                staffGroup.Description);
        }

        public StaffGroupDetail CreateDetail(StaffGroup staffGroup, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            return new StaffGroupDetail(
                staffGroup.GetRef(),
                staffGroup.Name,
                staffGroup.Description,
                CollectionUtils.Map<Staff, StaffSummary>(staffGroup.Members,
                                                         delegate (Staff staff)
                                                         {
                                                             return staffAssembler.CreateStaffSummary(staff, context);
                                                         })
                );
        }

        public void UpdateStaffGroup(StaffGroup group, StaffGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;

            group.Members.Clear();
            group.Members.AddAll(
                CollectionUtils.Map<StaffSummary, Staff>(detail.Members,
                                                         delegate(StaffSummary summary)
                                                         {
                                                             return context.Load<Staff>(summary.StaffRef, EntityLoadFlags.Proxy);
                                                         }));
        }
    }
}