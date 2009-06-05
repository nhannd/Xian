#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using NHibernate;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IPersistentStore"/>.
    /// </summary>
    public abstract class PersistentStore : IPersistentStore
    {
        private ISessionFactory _sessionFactory;
        private NHibernate.Cfg.Configuration _cfg;
        private ITransactionNotifier _transactionNotifier;

        public PersistentStore()
        {
        }

        #region IPersistentStore members

		public Version Version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

        public void Initialize()
        {
            Platform.Log(LogLevel.Info, "Initializing NHibernate subsystem...");

            // create the hibernate configuration
            _cfg = new NHibernate.Cfg.Configuration();

            // this will automatically read from the hibernate.xml.cfg file
            _cfg.Configure(ConfigFileLocation);

            Platform.Log(LogLevel.Debug, "NHibernate connection string: {0}", _cfg.Properties["connection.connection_string"]);

            // add each assembly to the hibernate configuration
            // this tells NHibernate to look for .hbm.xml embedded resources in these assemblies
			// TODO: we should only scan plugins that are tied to this PersistentStore, but there is currently no way to know this
			AssembliesHbmOrderer orderer = new AssembliesHbmOrderer(Platform.PluginManager.Plugins);
            orderer.AddToConfiguration(_cfg);
 
            // if a second-level cache has been specified
            if (_cfg.Properties.ContainsKey("hibernate.cache.provider_class"))
            {
                Platform.Log(LogLevel.Info, "NHibernate 2nd-level cache: {0}", _cfg.Properties["hibernate.cache.provider_class"]);

                // setup default caching strategies for all classes/collections that don't have one explicitly
                // specified in the mapping files
                CreateDefaultCacheStrategies();
            }

            // create the session factory
                _sessionFactory = _cfg.BuildSessionFactory();

            Platform.Log(LogLevel.Info, "NHibernate initialization complete.");
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

		protected abstract string ConfigFileLocation { get; }

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
                // only look at root classes that do not have an explicity cache strategy specified
                if (classMapping is RootClass && string.IsNullOrEmpty(classMapping.CacheConcurrencyStrategy))
                {
                    // cache EnumValue subclasses as nonstrict-read-write by default, since they are rarely modified
                    // (we might even be ok with a read-only strategy, but we'd have to try it out)
                    if (classMapping.MappedClass.IsSubclassOf(typeof(EnumValue)))
                    {
                        _cfg.SetCacheConcurrencyStrategy(classMapping.MappedClass, NHibernate.Cache.CacheFactory.NonstrictReadWrite);
                    }
                    else if(classMapping.MappedClass.IsSubclassOf(typeof(Entity)))
                    {
                        //JR: don't cache entities by default right now, because we don't have a distributed cache implementation
                        //_cfg.SetCacheConcurrencyStrategy(classMapping.MappedClass, NHibernate.Cache.CacheFactory.ReadWrite);
                    }

                }
            }


            //JR: don't cache collections by default right now, because we don't have a distributed cache implementation
            //foreach (NHibernate.Mapping.Collection collMapping in _cfg.CollectionMappings)
            //{
            //    if (string.IsNullOrEmpty(collMapping.CacheConcurrencyStrategy))
            //    {
            //        _cfg.SetCacheConcurrencyStrategy(collMapping.Role, NHibernate.Cache.CacheFactory.ReadWrite);
            //    }
            //}
        }
    }
}
