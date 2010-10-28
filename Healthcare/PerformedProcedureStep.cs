#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract class that roughly represents the notion of an IHE/DICOM General Purpose Performed Procedure Step (GP-PPS)
    /// </summary>
    public abstract class PerformedProcedureStep : PerformedStep
    {
		private IDictionary<string, string> _extendedProperties = new Dictionary<string, string>();


        public PerformedProcedureStep(Staff performingStaff)
            : base(new ProcedureStepPerformer(performingStaff))
        {
        }

        public PerformedProcedureStep(Staff performingStaff, DateTime? startTime)
            : base(new ProcedureStepPerformer(performingStaff), startTime)
        {
        }

        public PerformedProcedureStep()
        {
        }

		public virtual IDictionary<string, string> ExtendedProperties
        {
            get { return _extendedProperties; }
            protected set { _extendedProperties = value; }
        }

    }
}
