using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Expression;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Abstract base class for NHibernate implemenations of <see cref="IEntityBroker"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity class on which this broker operates</typeparam>
    /// <typeparam name="TSearchCriteria">The corresponding <see cref="SearchCriteria"/> class.</typeparam>
    public abstract class EntityBroker<TEntity, TSearchCriteria> : Broker, IEntityBroker<TEntity, TSearchCriteria>
        where TEntity : Entity
        where TSearchCriteria : SearchCriteria
    {
        public IList<TEntity> Match(TSearchCriteria criteria)
        {
            string baseHql = string.Format("from {0} x", typeof(TEntity).Name);
            HqlQuery query = HqlQuery.FromSearchCriteria(baseHql, "x", criteria);

            return MakeTypeSafe(ExecuteHql(query));
        }

        public TEntity Get(long oid)
        {
            TEntity entity = (TEntity)this.Context.Session.Load(typeof(TEntity), oid);

            // we don't want proxies for root objects, so ensure the object is actually materialized
            NHibernateUtil.Initialize(entity);

            return entity;
        }

        public void Store(TEntity entity)
        {
            if (this.Context.ReadOnly)
                throw new Exception();  //TODO elaborate

            this.Context.Session.SaveOrUpdate(entity);
        }

        public void Delete(TEntity entity)
        {
            if (this.Context.ReadOnly)
                 throw new Exception();  //TODO elaborate

             this.Context.Session.Delete(entity);
        }

        public void LoadRelated(TEntity entity, object property)
        {
            // if the entity is not part of the current session, re-attach
            if (!this.Context.Session.Contains(entity))
            {
                this.Context.Session.Lock(entity, LockMode.None);
            }

            NHibernateUtil.Initialize(property);
        }

        protected IList<TEntity> MakeTypeSafe(IList list)
        {
            return new ListWrapper<TEntity>(list);
        }

    }
}
