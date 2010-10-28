#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Workflow.Folders;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class RegistrationWorkflowFolder : WorklistFolder<RegistrationWorklistItemSummary, IRegistrationWorkflowService>
    {
        public RegistrationWorkflowFolder()
            : base(new RegistrationWorklistTable())
        {
        }
   }
}