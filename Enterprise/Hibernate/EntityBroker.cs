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
        public IList<TEntity> Find(TSearchCriteria criteria)
        {
            // use default page constraint
            return Find(criteria, new SearchResultPage());
        }

        public virtual IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page)
        {
            string baseHql = string.Format("from {0} x", typeof(TEntity).Name);
            HqlQuery query = HqlQuery.FromSearchCriteria(baseHql, "x", criteria, page);

            return MakeTypeSafe(ExecuteHql(query));
        }

        public virtual TEntity Find(long oid)
        {
            TEntity entity = (TEntity)this.Context.Session.Load(typeof(TEntity), oid);

            // we don't want proxies for root objects, so ensure the object is actually materialized
            NHibernateUtil.Initialize(entity);

            return entity;
        }

        public virtual long Count(TSearchCriteria criteria)
        {
            string baseHql = string.Format("select count(*) from {0} x", typeof(TEntity).Name);

            // construct the query object manually
            // for a "count" query, sort conditions and limits that may be present in the
            // criteria object must be ignored- therefore, only the conditions are added to the query
            HqlQuery query = new HqlQuery(baseHql);
            query.AddConditions(HqlCondition.FromSearchCriteria("x", criteria));
            IList results = ExecuteHql(query);

            // expect exactly one integer result
            return (long)results[0];
        }

        public virtual void Store(TEntity entity)
        {
            if (this.Context.ReadOnly)
                throw new Exception();  //TODO elaborate

            this.Context.Session.SaveOrUpdate(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (this.Context.ReadOnly)
                 throw new Exception();  //TODO elaborate

             this.Context.Session.Delete(entity);
        }

        public virtual void LoadRelated(TEntity entity, object property)
        {
            // if the entity is not part of the current session, re-attach
            if (!this.Context.Session.Contains(entity))
            {
                // NHibernate docs say to use LockMode.Read in this scenario - not sure why
                this.Context.Session.Lock(entity, LockMode.Read);
            }

            // if the property is not initialized, initialized it
            if (!NHibernateUtil.IsInitialized(property))
            {
                NHibernateUtil.Initialize(property);
            }
        }

        protected IList<TEntity> MakeTypeSafe(IList list)
        {
            return new ListWrapper<TEntity>(list);
        }

    }
}
