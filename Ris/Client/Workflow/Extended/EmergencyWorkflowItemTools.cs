#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	public class EmergencyOrdersConversationTool : PreliminaryDiagnosisConversationTool<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions, a => a is IClickAction && a.ActionID.EndsWith("pd")));
		}

		protected override string TemplatesXml
		{
			get
			{
				return PreliminaryDiagnosisSettings.Default.EmergencyTemplatesXml;
			}
		}

		protected override string SoftKeysXml
		{
			get
			{
				return PreliminaryDiagnosisSettings.Default.EmergencySoftKeysXml;
			}
		}
	}
}
