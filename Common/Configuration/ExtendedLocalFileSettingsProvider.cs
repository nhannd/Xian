#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    public class ExtendedLocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISharedApplicationSettingsProvider
    {
        private readonly LocalFileSettingsProvider _provider;

        public ExtendedLocalFileSettingsProvider(LocalFileSettingsProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Hack to allow shared settings migration to redirect to a different exe's config file.
        /// </summary>
        public static string ExeConfigFileName { get; set; }

        public override string ApplicationName
        {
            get { return _provider.ApplicationName; }
            set { _provider.ApplicationName = value; }
        }

        public override string Name
        {
            get
            {
                return _provider.Name;
            }
        }

        public override string Description
        {
            get { return _provider.Description; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            return _provider.GetPropertyValues(context, collection);
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _provider.Initialize(name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            _provider.SetPropertyValues(context, collection);
        }

        #region IApplicationSettingsProvider Members

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return _provider.GetPreviousVersion(context, property);
        }

        public void Reset(SettingsContext context)
        {
            _provider.Reset(context);
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            _provider.Upgrade(context, properties);
        }

        #endregion

        #region ISharedApplicationSettingsProvider Members

		public bool CanUpgradeSharedPropertyValues(SettingsContext context)
		{
			return true; //just let them get overwritten.
		}

    	public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            LocalFileSettingsProviderExtensions.UpgradeSharedPropertyValues(_provider, context, properties, previousExeConfigFilename, ExeConfigFileName);
        }

        public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            return LocalFileSettingsProviderExtensions.GetPreviousSharedPropertyValues(_provider, context, properties, previousExeConfigFilename);
        }

        public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            return LocalFileSettingsProviderExtensions.GetSharedPropertyValues(_provider, context, properties, ExeConfigFileName);
        }

        public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            LocalFileSettingsProviderExtensions.SetSharedPropertyValues(_provider, context, values, ExeConfigFileName);
        }

        #endregion
    }
}
