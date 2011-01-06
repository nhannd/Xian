#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public class ReportingPerformedProcedureStep : PerformedProcedureStep
    {
        public ReportingPerformedProcedureStep(Staff performingStaff)
            :base(performingStaff)
        {
        }

        public ReportingPerformedProcedureStep()
        {
        }

    }
}
