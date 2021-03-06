#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;

#if DEBUG
using HibernatingRhinos.Profiler.Appender.NHibernate;
using HibernatingRhinos.Profiler.Appender.StackTraces;
#endif

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// NHibernate implemenation of <see cref="IPersistentStore"/>.
	/// </summary>
	public abstract class PersistentStore : IPersistentStore
	{
		private ISessionFactory _sessionFactory;
		private Configuration _cfg;

		#region IPersistentStore members

		public Version Version
		{
			get
			{
                try
                {
                    using (IReadContext read = OpenReadContext())
                    {
                        IPersistentStoreVersionBroker broker = read.GetBroker<IPersistentStoreVersionBroker>();
                        PersistentStoreVersionSearchCriteria criteria = new PersistentStoreVersionSearchCriteria();
                        criteria.Major.SortDesc(0);
                        criteria.Minor.SortDesc(1);
                        criteria.Build.SortDesc(2);
                        criteria.Revision.SortDesc(3);

                        var versions = broker.Find(criteria);
                        if (versions.Count == 0)
                            return Assembly.GetExecutingAssembly().GetName().Version;                        

                        var version = CollectionUtils.FirstElement(versions);

                        return new Version(
                            int.Parse(version.Major),
                            int.Parse(version.Minor),
                            int.Parse(version.Build),
                            int.Parse(version.Revision));
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                    return Assembly.GetExecutingAssembly().GetName().Version;
                }
			}
		}

		/// <summary>
		/// Called by the framework to initialize the persistent store.
		/// </summary>
		public void Initialize()
		{
			Platform.Log(LogLevel.Info, "Initializing NHibernate subsystem...");

			// create the hibernate configuration
			_cfg = new Configuration();

			// this will automatically read from the app.config
			_cfg.Configure();

			Platform.Log(LogLevel.Debug, "NHibernate connection string: {0}", this.ConnectionString);

			// add each assembly to the hibernate configuration
			// this tells NHibernate to look for .hbm.xml embedded resources in these assemblies
			// TODO: we should only scan plugins that are tied to this PersistentStore, but there is currently no way to know this
			var orderer = new AssembliesHbmOrderer(Platform.PluginManager.Plugins);
			orderer.AddToConfiguration(_cfg);

			// setup default caching strategies for all classes/collections that don't have one explicitly
			// specified in the mapping files
			CreateDefaultCacheStrategies();

			// create the session factory
			_sessionFactory = _cfg.BuildSessionFactory();

#if DEBUG
			if (PersistentStoreProfilerSettings.Default.Enabled)
				InitializeProfiler();
#endif

			Platform.Log(LogLevel.Info, "NHibernate initialization complete.");
		}

		public void SetTransactionNotifier(ITransactionNotifier notifier)
		{
		}

		/// <summary>
		/// Obtains an <see cref="IReadContext"/> for use by the application to interact
		/// with this persistent store.
		/// </summary>
		/// <returns>a read context</returns>
		public IReadContext OpenReadContext()
		{
			return new ReadContext(this);
		}

		/// <summary>
		/// Obtains an <see cref="IUpdateContext"/> for use by the application to interact
		/// with this persistent store.
		/// </summary>
		/// <returns>a update context</returns>
		public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
		{
			return new UpdateContext(this, mode);
		}

		#endregion

		/// <summary>
		/// Gets the NHibernate Configuration object.
		/// </summary>
		internal Configuration Configuration
		{
			get { return _cfg; }
		}

		/// <summary>
		/// Gets the NHibernate connection string.
		/// </summary>
		internal string ConnectionString
		{
			get { return _cfg.GetProperty(NHibernate.Cfg.Environment.ConnectionString); }
		}


		/// <summary>
		/// Gets the NHibernate session factory.
		/// </summary>
		internal ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

#if DEBUG
		private static void InitializeProfiler()
		{
			Platform.Log(LogLevel.Info, "Initializing NHProf...");

			NHibernateProfiler.Initialize(new NHibernateAppenderConfiguration
			{
				// TODO: StackTraceFilters should not be set once issues with DynamicProxy2 and the stack trace are resolved
				StackTraceFilters = new IStackTraceFilter[0],
				DotNotFixDynamicProxyStackTrace = true
			});

			Platform.Log(LogLevel.Info, "NHProf initialization complete.");
		}
#endif

		/// <summary>
		/// Rather than explicitly specifying a cache-strategy in every class/collection mapping,
		/// create a default cache-strategy for all classes/collections that do not have
		/// an explicit strategy specified.  Then only classes/collections that require special treatment
		/// need to explicitly specify a cache strategy.
		/// </summary>
		private void CreateDefaultCacheStrategies()
		{
			foreach (var classMapping in _cfg.ClassMappings)
			{
				// only look at root classes that do not have an explicity cache strategy specified
				if (classMapping is RootClass && string.IsNullOrEmpty(classMapping.CacheConcurrencyStrategy))
				{
					// cache EnumValue subclasses as nonstrict-read-write by default, since they are rarely modified
					// (we might even be ok with a read-only strategy, but we'd have to try it out)
					if (classMapping.MappedClass.IsSubclassOf(typeof(EnumValue)))
					{
						_cfg.SetCacheConcurrencyStrategy(classMapping.MappedClass.FullName, NHibernate.Cache.CacheFactory.NonstrictReadWrite);
					}
					else if (classMapping.MappedClass.IsSubclassOf(typeof(Entity)))
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
