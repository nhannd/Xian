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
using ClearCanvas.Enterprise.Core.Modelling;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// InterpretationStep entity
    /// </summary>
	public partial class InterpretationStep : ReportingProcedureStep
	{
        public InterpretationStep(Procedure procedure)
            :base(procedure, null)
        {
		}

        public InterpretationStep(ReportingProcedureStep previousStep)
            :base(previousStep)
        {
			CustomInitialize();
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		protected override void LinkProcedure(Procedure procedure)
		{
			if(this.Report == null)
				throw new WorkflowException("This step must be associated with a Report before procedures can be linked.");

			this.Report.LinkProcedure(procedure);
		}

        public override string Name
        {
            get { return "Interpretation"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if(newState == ActivityStatus.CM)
            {
                if (this.ReportPart == null)
                    throw new WorkflowException("This ReportingStep does not have an associated ReportPart.");
            }

            base.OnStateChanged(previousState, newState);
        }

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new InterpretationStep(this);
		}
	}
}
