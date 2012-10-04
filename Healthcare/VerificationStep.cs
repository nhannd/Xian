#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using System.Xml.Linq;

namespace ClearCanvas.Healthcare
{


    /// <summary>
    /// VerificationStep entity
    /// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class VerificationStep : ReportingProcedureStep
	{

        public VerificationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
        {
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
            get { return "Verification"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if (newState == ActivityStatus.CM)
                this.ReportPart.Verifier = this.PerformingStaff;

            base.OnStateChanged(previousState, newState);
        }

		public override ProcedureStep Reassign(Staff performer)
		{
			var reassign = base.Reassign(performer).Downcast<VerificationStep>();

			// When reassigning a verification step to another staff, we should reassign the supervisor as well
			// so the report part will be reviewed by the appropriate staff radiologist
			if (reassign.ReportPart != null && reassign.ReportPart.Supervisor != null)
				reassign.ReportPart.Supervisor = performer;

			return reassign;
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new VerificationStep(this);
		}

		private static IValidationRuleSet GetValidationRules()
		{
			return new ValidationRuleSet(new[]
			{
				new ValidationRule<VerificationStep>(ValidateReportTextNotBlank)
			});
		}

		private static TestResult ValidateReportTextNotBlank(VerificationStep step)
    	{
			// none of this applies unless we're transitioning into the completed state
			if (step.State != ActivityStatus.CM)
				return new TestResult(true);

			// check for a non-empty ReportContent property
    		string content;
    		if(step.ReportPart.ExtendedProperties.TryGetValue("ReportContent", out content) || string.IsNullOrEmpty(content))
				return new TestResult(false, SR.MessageValidateVerifiedReportIsNotBlank);

			// attempt to parse reportContent property as XML, to check for content
			try
			{
				var report = XDocument.Parse(content).Elements("Report").FirstOrDefault();
				if (report == null || string.IsNullOrEmpty(report.Value))
					return new TestResult(false, SR.MessageValidateVerifiedReportIsNotBlank);
			}
			catch(Exception)
			{
				// if we can't parse it, there isn't much we can do but assume it is valid
			}
			return new TestResult(true);
		}
	}
}