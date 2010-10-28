#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class WorklistAssembler
    {
        public WorklistSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
        	return new WorklistSummary(
        		worklist.GetRef(),
        		worklist.Name,
        		worklist.Description,
        		worklist.ClassName,
				Worklist.GetCategory(worklist.GetClass()),
        		Worklist.GetDisplayName(worklist.GetClass()),
				worklist.Owner.IsStaffOwner ? staffAssembler.CreateStaffSummary(worklist.Owner.Staff, context) : null,
                worklist.Owner.IsGroupOwner ? groupAssembler.CreateSummary(worklist.Owner.Group) : null);
        }
    }
}
