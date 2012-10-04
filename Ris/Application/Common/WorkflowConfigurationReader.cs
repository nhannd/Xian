#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Ris.Application.Common
{
	public class WorkflowConfigurationReader
	{
		private readonly WorkflowSettings _settings;

		public WorkflowConfigurationReader()
		{
			_settings = new WorkflowSettings();
		}

		public bool EnableTranscriptionWorkflow
		{
			get { return _settings.EnableTranscriptionWorkflow; }
		}

		public bool EnableInterpretationReviewWorkflow
		{
			get { return _settings.EnableInterpretationReviewWorkflow; }
		}

		public bool EnableTranscriptionReviewWorkflow
		{
			get { return _settings.EnableTranscriptionReviewWorkflow; }
		}

		public bool EnableVisitWorkflow
		{
			get { return _settings.EnableVisitWorkflow; }
		}

		public bool AllowUnscheduledProcedures
		{
			get { return _settings.AllowUnscheduledProcedures; }
		}

		public bool AllowMultipleInformationAuthorities
		{
			get { return _settings.AllowMultipleInformationAuthorities; }
		}
	}
}
