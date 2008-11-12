using System.Configuration;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
	[SettingsGroupDescription("Configures behaviour of reporting workflow.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ReportingWorkflowSettings
	{
		///<summary>
		/// Public constructor.  Server-side settings classes should be instantiated via constructor rather
		/// than using the <see cref="ReportingWorkflowSettings.Default"/> property to avoid creating a static instance.
		///</summary>
		public ReportingWorkflowSettings()
		{
		}
	}
}
