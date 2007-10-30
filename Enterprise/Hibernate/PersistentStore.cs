#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
            AssembliesHbmOrderer orderer = new AssembliesHbmOrderer(Platform.PluginManager.Plugins);
            orderer.AddToConfiguration(_cfg);
 
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
            return new ReadContext(this);
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            return new UpdateContext(this, mode);
        }

        #endregion

        internal ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        internal ITransactionNotifier TransactionNotifier
        {
            get { return _transactionNotifier; }
        }

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
                if (classMapping is RootClass && string.IsNullOrEmpty(classMapping.CacheConcurrencyStrategy))
                {
                    _cfg.SetCacheConcurrencyStrategy(classMapping.MappedClass, NHibernate.Cache.CacheFactory.ReadWrite);
                }
            }

            foreach (NHibernate.Mapping.Collection collMapping in _cfg.CollectionMappings)
            {
                if (string.IsNullOrEmpty(collMapping.CacheConcurrencyStrategy))
                {
                    _cfg.SetCacheConcurrencyStrategy(collMapping.Role, NHibernate.Cache.CacheFactory.ReadWrite);
                }
            }
        }

    }
}
