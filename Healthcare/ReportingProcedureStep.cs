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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        private ReportPart _reportPart;

        public ReportingProcedureStep()
        {
        }

        public ReportingProcedureStep(Procedure procedure, ReportPart reportPart)
            :base(procedure)
        {
            _reportPart = reportPart;
        }

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            : this(previousStep.Procedure, previousStep.ReportPart)
        {
        }

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return false; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.MaxValue; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			if(_reportPart != null && _reportPart.Report != null)
			{
				return CollectionUtils.Select(_reportPart.Report.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ReportPart"/> that this step targets, or null if there is no associated report part.
		/// </summary>
        public virtual ReportPart ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

		/// <summary>
		/// Gets the <see cref="Report"/> that this step is associated with, or null if not associated.
		/// </summary>
    	public virtual Report Report
    	{
			get { return _reportPart == null ? null : _reportPart.Report; }
    	}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no report
			if(this.Report == null)
				return false;

			// relatives must be reporting steps
			if (!step.Is<ReportingProcedureStep>())
				return false;

			// check if tied to same report
			ReportingProcedureStep that = step.As<ReportingProcedureStep>();
			return that.Report != null && Equals(this.Report, that.Report);
		}
	}
}
