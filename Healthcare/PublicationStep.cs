#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// PublicationStep entity
	/// </summary>
	public class PublicationStep : ReportingProcedureStep
	{
		private int _failureCount;
		private DateTime? _lastFailureTime;

		public PublicationStep()
		{
		}

		public PublicationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
		{
			_failureCount = 0;
			_lastFailureTime = null;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public override string Name
		{
			get { return "Publication"; }
		}

		public virtual int FailureCount
		{
			get { return _failureCount; }
		}

		public virtual DateTime? LastFailureTime
		{
			get { return _lastFailureTime; }
		}

		public virtual void Fail()
		{
			_failureCount++;
			_lastFailureTime = Platform.Time;
		}

		protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
		{
			// complete the report part when publication is complete
			if (newState == ActivityStatus.CM)
			{
				this.ReportPart.Complete();

				// if step corresponds to the initial report (not an addendum), mark procedure(s) as
				// being complete
				if (this.ReportPart.Index == 0)
				{
					foreach (Procedure procedure in this.AllProcedures)
					{
						procedure.Complete((DateTime) this.EndTime);
					}
				}
			}

			base.OnStateChanged(previousState, newState);
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new PublicationStep(this);
		}
	}
}