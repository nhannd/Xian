using System.Configuration;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	[SettingsGroupDescription("Settings that configure behaviour of the order entry service.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class OrderEntrySettings
	{
		public OrderEntrySettings()
		{
		}
	}
}
