using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
    [ExtensionPoint]
    public class EnterpriseConfigurationStoreExtensionPoint : ExtensionPoint<IConfigurationStore>
    {
    }

    [ExtensionPoint]
    public class LocalConfigurationStoreExtensionPoint : ExtensionPoint<IConfigurationStore>
    {
    }


    /// <summary>
    /// This class is the standard settings provider that should be used by all settings classes that operate
    /// within the ClearCanvas framework.  Internally, this class will delegate the storage of settings between
    /// the local file system and an implemetation of <see cref="EnterpriseConfigurationStoreExtensionPoint"/>,
    /// if an extension is found.
    /// </summary>
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
                _sourceProvider = GetConfigurationStoreProvider(new EnterpriseConfigurationStoreExtensionPoint());
            }
            catch (NotSupportedException)
            {
                Platform.Log(SR.LogEnterpriseConfigurationStoreNotFound, LogLevel.Info);

                try
                {
                    // try for a local configuration store
                    _sourceProvider = GetConfigurationStoreProvider(new LocalConfigurationStoreExtensionPoint());
                }
                catch (NotSupportedException)
                {
                    Platform.Log(SR.LogLocalConfigurationStoreNotFound, LogLevel.Info);
                    
                    // default to LocalFileSettingsProvider as a last resort
                    _sourceProvider = new LocalFileSettingsProvider();
                }
            }

            // init source provider
            // according to sample implementations, use the application name here
            _sourceProvider.Initialize(this.ApplicationName, config);
            base.Initialize(this.ApplicationName, config);
        }

        private SettingsProvider GetConfigurationStoreProvider(IExtensionPoint configStoreExtPoint)
        {
            IConfigurationStore ecs = (IConfigurationStore)configStoreExtPoint.CreateExtension();
            return new ConfigurationStoreSettingsProvider(ecs);
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
            if (_sourceProvider is IApplicationSettingsProvider)
            {
                return (_sourceProvider as IApplicationSettingsProvider).GetPreviousVersion(context, property);
            }
            else
            {
                // fail silently as per MSDN 
                return new SettingsPropertyValue(property);
            }
        }

        public void Reset(SettingsContext context)
        {
            if (_sourceProvider is IApplicationSettingsProvider)
            {
                (_sourceProvider as IApplicationSettingsProvider).Reset(context);
            }
            else
            {
                // fail silently as per MSDN 
            }
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            if (_sourceProvider is IApplicationSettingsProvider)
            {
                (_sourceProvider as IApplicationSettingsProvider).Upgrade(context, properties);
            }
            else
            {
                // fail silently as per MSDN 
            }
        }

        #endregion
    }
}
