using System.Configuration;

namespace ClearCanvas.Healthcare
{
	[SettingsGroupDescription("Settings that configure the behaviour of the procedure builder.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ProcedureBuilderSettings
	{
	}
}
