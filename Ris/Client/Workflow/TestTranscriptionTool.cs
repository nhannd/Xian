using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/TEST", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/TEST", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class TestTranscriptionTool : TranscriptionWorkflowItemTool
	{
		[DataContract]
		class ReportContent : DataContractBase
		{
			/// <summary>
			/// The free-text component of the report.
			/// </summary>
			[DataMember]
			public string ReportText;

			/// <summary>
			/// A structured-report component, when applicable, otherwise this property will be null.
			/// </summary>
			[DataMember]
			public XmlDocument StructuredReport;
		}

		public TestTranscriptionTool()
			: base("StartTranscription")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Transcription.CompletedFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					StartTranscriptionResponse startResponse = service.StartTranscription(new StartTranscriptionRequest(item.ProcedureStepRef));

					LoadTranscriptionForEditResponse loadResponse = service.LoadTranscriptionForEdit(new LoadTranscriptionForEditRequest(startResponse.TranscriptionStepRef));
					Dictionary<string, string> extendedProperties = loadResponse.Report.GetPart(loadResponse.ReportPartIndex).ExtendedProperties;
					ReportContent content = JsmlSerializer.Deserialize<ReportContent>(extendedProperties[ReportPartDetail.ReportContentKey]);
					content.ReportText = content.ReportText + " FAKE TRANSCRIPTION TEXT";
					extendedProperties[ReportPartDetail.ReportContentKey] = JsmlSerializer.Serialize(content, "Report", false);

					service.SaveTranscription(new SaveTranscriptionRequest(startResponse.TranscriptionStepRef, extendedProperties));
				});

			this.Context.InvalidateFolders(typeof(Folders.Transcription.DraftFolder));

			return true;
		}
	}
}