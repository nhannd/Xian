using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Enterprise.Configuration
{
    public class EnterpriseSettingsProvider : SettingsProvider
    {
        private string _appName;

        public EnterpriseSettingsProvider()
        {
        }

        public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            Type settingsClass = (Type)context["SettingsClassType"];

            // TODO: use assembly attributes to determine app and version
            string appName = "Foo";
            string appVersion = "Bar";
            string groupName = (string)context["GroupName"];
            string settingsKey = (string)context["SettingsKey"];

            IConfigurationService service = ApplicationContext.GetService<IConfigurationService>();
            IList<ConfigSetting> configSettings = service.LoadConfigSettings(appName, appVersion, groupName, settingsKey);

            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region IApplicationSettingsProvider Members

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Reset(SettingsContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
