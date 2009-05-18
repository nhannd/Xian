using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// ConfigurationSettingsGroup entity
    /// </summary>
	public partial class ConfigurationSettingsGroup : ClearCanvas.Enterprise.Core.Entity
	{
        public static ConfigurationSettingsGroupSearchCriteria GetCriteria(SettingsGroupDescriptor descriptor)
        {
            ConfigurationSettingsGroupSearchCriteria where = new ConfigurationSettingsGroupSearchCriteria();
            where.Name.EqualTo(descriptor.Name);
            where.VersionString.EqualTo(VersionUtils.ToPaddedVersionString(descriptor.Version, false, false));
            return where;
        }

        public virtual void UpdateFromDescriptor(SettingsGroupDescriptor descriptor)
        {
            _assemblyQualifiedTypeName = descriptor.AssemblyQualifiedTypeName;
            _name = descriptor.Name;
            _versionString = VersionUtils.ToPaddedVersionString(descriptor.Version, false, false);
            _description = descriptor.Description;
            _hasUserScopedSettings = descriptor.HasUserScopedSettings;
        }

        public virtual SettingsGroupDescriptor GetDescriptor()
        {
            return new SettingsGroupDescriptor(
                _name,
                VersionUtils.FromPaddedVersionString(_versionString),
                _description,
                _assemblyQualifiedTypeName,
                _hasUserScopedSettings);
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}