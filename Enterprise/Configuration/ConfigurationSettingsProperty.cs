using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Configuration;


namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// ConfigurationSettingsProperty component
    /// </summary>
	public partial class ConfigurationSettingsProperty
	{
		/// <summary>
		/// Updates this instance from the specified descriptor.
		/// </summary>
		/// <param name="descriptor"></param>
		public void UpdateFromDescriptor(SettingsPropertyDescriptor descriptor)
        {
            _name = descriptor.Name;
            _typeName = descriptor.TypeName;
            _scope = descriptor.Scope.ToString();
            _description = descriptor.Description;
            _defaultValue = descriptor.DefaultValue;
        }

		/// <summary>
		/// Returns a descriptor populated from this instance.
		/// </summary>
		/// <returns></returns>
		public SettingsPropertyDescriptor GetDescriptor()
        {
            return new SettingsPropertyDescriptor(
                _name,
                _typeName,
                _description,
                (SettingScope)Enum.Parse(typeof(SettingScope), _scope),
                _defaultValue);
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