using System.Configuration;

namespace ClearCanvas.Ris.Application.Services.Admin.PatientAdmin
{
	[SettingsGroupDescription("Settings that configure behaviour of the patient administration service.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class PatientAdminSettings
	{
	}
}
