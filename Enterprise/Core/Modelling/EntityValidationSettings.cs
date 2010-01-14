using System.Configuration;

namespace ClearCanvas.Enterprise.Core.Modelling
{

	[SettingsGroupDescription("Settings that affect the behaviour of custom entity validation rules.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class EntityValidationSettings
	{
		///<summary>
		/// Public constructor.  Server-side settings classes should be instantiated via constructor rather
		/// than using the <see cref="EntityValidationSettings.Default"/> property to avoid creating a static instance.
		///</summary>
		public EntityValidationSettings()
		{
		}
	}
}
