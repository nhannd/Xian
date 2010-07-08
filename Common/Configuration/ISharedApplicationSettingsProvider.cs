using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    public interface ISharedApplicationSettingsProvider
    {
        //void ResetSharedPropertyValues();

        void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties,
                                         string previousExeConfigFilename);

    	SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context,
    	                                                                SettingsPropertyCollection properties,
    	                                                                string previousExeConfigFilename);
		       
        SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties);

        void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values);
    }
}