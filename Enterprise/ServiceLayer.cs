using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all service layer classes.
    /// </summary>
    public abstract class ServiceLayer : IServiceLayer
    {
        private static Dictionary<Type, object> _mapEnumTables;

        static ServiceLayer()
        {
            _mapEnumTables = new Dictionary<Type, object>();
        }

        private IPersistenceContext _currentContext;
        private ISession _session;

        protected ServiceLayer()
        {
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }

        protected ISession Session
        {
            get { return _session; }
        }

        /// <summary>
        /// Allows the current context to be set by the framework
        /// </summary>
        public IPersistenceContext CurrentContext
        {
            get { return _currentContext; }
            internal set { _currentContext = value; }
        }

        /// <summary>
        /// Provides subclass access to the current read context, if one is available 
        /// </summary>
        protected IReadContext ReadContext
        {
            get
            {
                if (CurrentContext == null || !(CurrentContext is IReadContext))
                    throw new Exception("No read context available");   // todo elaborate
                return (IReadContext)CurrentContext;
            }
        }

        /// <summary>
        /// Provides subclass access to the current update context, if one is available 
        /// </summary>
        protected IUpdateContext UpdateContext
        {
            get
            {
                if (CurrentContext == null || !(CurrentContext is IUpdateContext))
                    throw new Exception("No update context available");   // todo elaborate
                return (IUpdateContext)CurrentContext;
            }
        }
/*
        /// <summary>
        /// Obtains the domain enumeration table for the specified C# enum type.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        protected EnumTable<e, E> GetEnumTable<e, E, TBrokerInterface>()
            where e : struct
            where E : EnumValue<e>
            where TBrokerInterface : IEnumBroker<e, E>
        {
            if(!_mapEnumTables.ContainsKey(typeof(e)))
            {
                using (IReadContext rctx = _session.GetReadContext())
                {
                    TBrokerInterface broker = rctx.GetBroker<TBrokerInterface>();
                    _mapEnumTables.Add(typeof(e), broker.Load());
                }
            }
            return (EnumTable<e, E>)_mapEnumTables[typeof(e)];
        }
 */
    }
}
