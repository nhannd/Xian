using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    [ExtensionPoint]
    public class EnterpriseConfigurationStoreExtensionPoint : ExtensionPoint<IEnterpriseConfigurationStore>
    {
    }


    public class StandardSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private string _appName;
        private SettingsProvider _sourceProvider;


        public StandardSettingsProvider()
        {
            // according to MSDN recommendation, use the name of the executing assembly here
            _appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
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

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

            // obtain a source provider, using an enterprise one if available
            try
            {
                IEnterpriseConfigurationStore ecs = (IEnterpriseConfigurationStore)
                            (new EnterpriseConfigurationStoreExtensionPoint()).CreateExtension();

                _sourceProvider = ecs.GetSettingsProvider();

            }
            catch (NotSupportedException)
            {
                Platform.Log(SR.LogEnterpriseConfigurationStoreNotFound, LogLevel.Info);
                _sourceProvider = new LocalFileSettingsProvider();
            }

            // init source provider
            // according to sample implementations, use the application name here
            _sourceProvider.Initialize(this.ApplicationName, config);
            base.Initialize(this.ApplicationName, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            return _sourceProvider.GetPropertyValues(context, props);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            _sourceProvider.SetPropertyValues(context, settings);
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
