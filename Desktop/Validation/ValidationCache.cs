using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Caches custom validation rules for application components.
    /// </summary>
    public static class ValidationCache
    {
        private static readonly Dictionary<Type, string> _rulesCache = new Dictionary<Type, string>();
        private static readonly IConfigurationStore _configStore;
        private static readonly object _syncLock = new object();

        static ValidationCache()
        {
            try
            {
                _configStore = ConfigurationStoreFactory.GetDefaultStore();
            }
            catch (NotSupportedException e)
            {
                Platform.Log(LogLevel.Debug, e);
                _configStore = null;
            }
        }

        public static string GetValidationRulesXml(Type applicationComponentClass)
        {
            lock (_syncLock)
            {
                // if there is no config store, there are no rules
                if (_configStore == null)
                    return null;

                // try to get it from the cache
                string rulesXml;
                if (_rulesCache.TryGetValue(applicationComponentClass, out rulesXml))
                    return rulesXml;

                // load and cache it
                rulesXml = LoadValidationDocument(applicationComponentClass);
                _rulesCache.Add(applicationComponentClass, rulesXml);

                return rulesXml;
            }
        }

        public static void Invalidate(Type applicationComponentClass)
        {
            lock (_syncLock)
            {
                if (_rulesCache.ContainsKey(applicationComponentClass))
                    _rulesCache.Remove(applicationComponentClass);
            }
        }

        private static string LoadValidationDocument(Type applicationComponentClass)
        {
            try
            {
                string documentName = string.Format("{0}.val.xml", applicationComponentClass.FullName);
                TextReader reader = _configStore.GetDocument(documentName, applicationComponentClass.Assembly.GetName().Version, null, null);
                return reader.ReadToEnd();

            }
            catch (ConfigurationDocumentNotFoundException e)
            {
                Platform.Log(LogLevel.Debug, e);
                return null;
            }
        }
    }
}
