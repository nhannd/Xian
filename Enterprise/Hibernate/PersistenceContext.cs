#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;

using NHibernate;
using System.Data;
using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Defines the extension point for all NHibernate broker classes.
    /// </summary>
    [ExtensionPoint()]
    public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
    {
    }

    /// <summary>
    /// Abstract base class for NHibernate persistence context implementations.
    /// </summary>
    public abstract class PersistenceContext : IPersistenceContext
    {
        private PersistentStore _persistentStore;
        private ISession _session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="readOnly"></param>
        internal PersistenceContext(PersistentStore persistentStore)
        {
            _persistentStore = persistentStore;
        }

        #region IPersistenceContext members

        /// <summary>
        /// Gets a broker that implements the specified interface.
        /// </summary>
        /// <typeparam name="TBrokerInterface"></typeparam>
        /// <returns></returns>
        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            return (TBrokerInterface)GetBroker(typeof(TBrokerInterface));
        }

        public object GetBroker(Type brokerInterface)
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            IPersistenceBroker broker = (IPersistenceBroker)xp.CreateExtension(new TypeExtensionFilter(brokerInterface));
            broker.SetContext(this);
            return broker;
        }

        /// <summary>
        /// Locks the specified entity into this context.
        /// </summary>
        /// <param name="entity"></param>
        public void Lock(Entity entity)
        {
            LockCore(entity, DirtyState.Clean);
        }

        /// <summary>
        /// Locks the specified entity into this context with the specified dirty state.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dirtyState"></param>
        public void Lock(Entity entity, DirtyState dirtyState)
        {
            LockCore(entity, dirtyState);
        }

        /// <summary>
        /// Loads the specified entity into this context using the default load flags for the context.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        public TEntity Load<TEntity>(EntityRef entityRef)
            where TEntity : Entity
        {
            return this.Load<TEntity>(entityRef, this.DefaultEntityLoadFlags);
        }

        /// <summary>
        /// Loads the specified entity into this context using the specified load flags.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityRef"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags)
            where TEntity : Entity
        {
            try
            {
                // use a proxy if EntityLoadFlags.Proxy is specified and EntityLoadFlags.CheckVersion is not specified (CheckVersion overrides Proxy)
                bool useProxy = (flags & EntityLoadFlags.CheckVersion) == 0 && (flags & EntityLoadFlags.Proxy) == EntityLoadFlags.Proxy;
                Entity entity = (Entity)Load(EntityRefUtils.GetClass(entityRef), EntityRefUtils.GetOID(entityRef), useProxy);

                // check version if necessary
                if ((flags & EntityLoadFlags.CheckVersion) == EntityLoadFlags.CheckVersion && !EntityRefUtils.GetVersion(entityRef).Equals(entity.Version))
                    throw new EntityVersionException(EntityRefUtils.GetOID(entityRef), null);

                return (TEntity)entity;

            }
            catch (ObjectNotFoundException hibernateException)
            {
                // note that we will only get here if useProxy == false in the above block
                // if the entity is proxied, verification of its existence is deferred until the proxy is realized
                throw new EntityNotFoundException(hibernateException);
            }
        }

        /// <summary>
        /// Synchronizes the state of the context with the persistent store, ensuring that any new entities
        /// have OIDs generated.
        /// </summary>
        public void SynchState()
        {
            try
            {
                if (_session != null)
                {
                    SynchStateCore();
                }
            }
            catch (Exception e)
            {
                HandleHibernateException(e, SR.ExceptionSynchState);
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Public members

        /// <summary>
        /// Allows a broker to create an ADO.NET command, rather than using NHibernate.  The command
        /// will execute on the same connection and within the same transaction
        /// as any other operation on this context.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IDbCommand CreateSqlCommand(string sql)
        {
            IDbCommand cmd = this.Session.Connection.CreateCommand();
            cmd.CommandText = sql;
            this.Session.Transaction.Enlist(cmd);

            return cmd;
        }

        /// <summary>
        /// Allows a broker to create an NHibernate query.
        /// </summary>
        /// <param name="hql"></param>
        /// <returns></returns>
        public IQuery CreateHibernateQuery(string hql)
        {
            return this.Session.CreateQuery(hql);
        }

        /// <summary>
        /// Allows a broker to load a named HQL query stored in a *.hbm.xml file.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQuery GetNamedHqlQuery(string name)
        {
            return this.Session.GetNamedQuery(name);
        }

        #endregion

        #region Protected abstract members

        /// <summary>
        /// Default <see cref="EntityLoadFlags"/> to be used by this context.
        /// </summary>
        protected abstract EntityLoadFlags DefaultEntityLoadFlags { get; }

        /// <summary>
        /// Factory method to create the NHibernate session.
        /// </summary>
        /// <returns></returns>
        protected abstract ISession CreateSession();

        /// <summary>
        /// Implementation of SynchState logic.
        /// </summary>
        protected abstract void SynchStateCore();

        /// <summary>
        /// Implementation of Lock logic.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dirtyState"></param>
        protected abstract void LockCore(Entity entity, DirtyState dirtyState);

        /// <summary>
        /// True if this context is read-only.
        /// </summary>
        internal abstract bool ReadOnly { get; }

        #endregion

        #region Helper methods

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        protected void CommitTransaction()
        {
            if (_session != null)
            {
                _session.Transaction.Commit();
                _session.Close();
                _session = null;
            }
        }

        /// <summary>
        /// Rollsback the current transaction.
        /// </summary>
        protected void RollbackTransaction()
        {
            if (_session != null)
            {
                _session.Transaction.Rollback();
                _session.Close();
                _session = null;
            }
        }

        /// <summary>
        /// Loads the specified enum value into this context.
        /// </summary>
        /// <param name="enumValueClass"></param>
        /// <param name="code"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        internal EnumValue LoadEnumValue(Type enumValueClass, string code, bool proxy)
        {
            try
            {
                return (EnumValue)Load(enumValueClass, code, proxy);
            }
            catch (ObjectNotFoundException hibernateException)
            {
                // note that we will only get here if proxy == false in the above block
                // if the entity is proxied, verification of its existence is deferred until the proxy is realized
                throw new EnumValueNotFoundException(enumValueClass, code, hibernateException);
            }
        }

        /// <summary>
        /// Loads the specified persistent object into this context.
        /// </summary>
        /// <param name="persistentClass"></param>
        /// <param name="oid"></param>
        /// <param name="useProxy"></param>
        /// <returns></returns>
        private object Load(Type persistentClass, object oid, bool useProxy)
        {
            return useProxy ?
                this.Session.Load(persistentClass, oid, LockMode.None)
                : this.Session.Get(persistentClass, oid);
        }

        /// <summary>
        /// Wraps an NHibernate exception and rethrows it
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        protected void HandleHibernateException(Exception e, string message)
        {
            if (e is StaleObjectStateException)
            {
                throw new EntityVersionException((e as StaleObjectStateException).Identifier, e);
            }
            else if (e is ObjectNotFoundException)
            {
                throw new EntityNotFoundException(e);
            }
            else if (e is EntityValidationException)
            {
                // don't wrap EntityValidationException, which we throw from the interceptor
                throw e;
            }
            ///TODO any other specific kinds of exceptions we need to consider?
            else
            {
                throw new PersistenceException(message, e);
            }
        }

        /// <summary>
        /// Implementation of the Dispose pattern
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_session != null)
                {
                    _session.Close();
                    _session = null;
                }
            }
        }


        #endregion

        #region Protected and Internal properties

        /// <summary>
        /// Gets the NHibernate Session object, instantiating it if it does not already exist.
        /// </summary>
        internal ISession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = CreateSession();
                    _session.BeginTransaction();
                }

                return _session;
            }
        }

        /// <summary>
        /// Gets the persistent store with which this context is associated.
        /// </summary>
        internal PersistentStore PersistentStore
        {
            get { return _persistentStore; }
        }

        #endregion
    }
}
