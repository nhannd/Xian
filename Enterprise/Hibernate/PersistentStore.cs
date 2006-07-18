using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
        private ISessionFactory _sessionFactory;
        private Configuration _cfg;

        public PersistentStore()
        {
        }

        public void Initialize()
        {
            // create the hibernate configuration
            _cfg = new Configuration();

            // this will automatically read from the hibernate.xml.cfg file
            _cfg.Configure();
                        
            // build the set of all assemblies containing Broker extensions
            // the assumption is that these assemblies also contain the .hbm.xml mapping files
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            ExtensionInfo[] extensions = xp.ListExtensions();
            HybridSet assemblies = new HybridSet();
            foreach (ExtensionInfo extension in extensions)
            {
                assemblies.Add(extension.ExtensionClass.Assembly);
            }

            // add each assembly to the hibernate configuration
            // this tells NHibernate to look for .hbm.xml embedded resources in these assemblies
            foreach (Assembly asm in assemblies)
            {
                _cfg.AddAssembly(asm);
            }

            // create the session factory
            _sessionFactory = _cfg.BuildSessionFactory();
        }

        public IReadContext GetReadContext()
        {
            return new ReadContext(_sessionFactory);
        }

        public IUpdateContext GetUpdateContext()
        {
            return new UpdateContext(_sessionFactory);
        }

        public Configuration Configuration
        {
            get { return _cfg; }
        }

        public IDictionary Metadata
        {
            get { return _sessionFactory.GetAllClassMetadata(); }
        }
    }
}
