using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;
using NHibernate.Mapping;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private ISessionFactory _sessionFactory;
        private NHibernate.Cfg.Configuration _cfg;

        private ITransactionNotifier _transactionNotifier;

        public PersistentStore()
        {
        }

        #region IPersistentStore members

        public void Initialize()
        {
            // create the hibernate configuration
            _cfg = new NHibernate.Cfg.Configuration();

            // this will automatically read from the hibernate.xml.cfg file
            _cfg.Configure();

            // add each assembly to the hibernate configuration
            // this tells NHibernate to look for .hbm.xml embedded resources in these assemblies
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                _cfg.AddAssembly(plugin.Assembly);
            }

            // if a second-level cache has been specified
            if (_cfg.Properties.Contains("hibernate.cache.provider_class"))
            {
                // setup default caching strategies for all classes/collections that don't have one explicitly
                // specified in the mapping files
                CreateDefaultCacheStrategies();
            }

            // create the session factory
            _sessionFactory = _cfg.BuildSessionFactory();
        }

        public void SetTransactionNotifier(ITransactionNotifier notifier)
        {
            _transactionNotifier = notifier;
        }

        public IReadContext OpenReadContext()
        {
            return new ReadContext(_sessionFactory);
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            return new UpdateContext(_sessionFactory, _transactionNotifier, mode);
        }

        #endregion


        public NHibernate.Cfg.Configuration Configuration
        {
            get { return _cfg; }
        }

        public IDictionary Metadata
        {
            get { return _sessionFactory.GetAllClassMetadata(); }
        }

        /// <summary>
        /// Rather than explicitly specifying a cache-strategy in every class/collection mapping,
        /// create a default cache-strategy for all classes/collections that do not have
        /// an explicit strategy specified.  Then only classes/collections that require special treatment
        /// need to explicitly specify a cache strategy.
        /// </summary>
        private void CreateDefaultCacheStrategies()
        {
            foreach (PersistentClass classMapping in _cfg.ClassMappings)
            {
                if (classMapping is RootClass && classMapping.Cache == null)
                {
                    _cfg.SetCacheConcurrencyStrategy(classMapping.MappedClass, new NHibernate.Cache.ReadWriteCache());
                }
            }

            foreach (NHibernate.Mapping.Collection collMapping in _cfg.CollectionMappings)
            {
                if (collMapping.Cache == null)
                {
                    _cfg.SetCacheConcurrencyStrategy(collMapping.Role, new NHibernate.Cache.ReadWriteCache());
                }
            }
        }

    }
}
