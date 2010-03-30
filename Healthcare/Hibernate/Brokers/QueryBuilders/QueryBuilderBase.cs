using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	public abstract partial class QueryBuilderBase : IQueryBuilder
	{


		#region Implementation of IQueryBuilder

		public abstract void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args);

		public virtual void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			QueryBuilderHelpers.AddCriteriaToQuery(HqlConstants.WorklistItemQualifier, args.Criteria, query, RemapHqlExpression);

			// modify the query to workaround some NHibernate bugs
			QueryBuilderHelpers.NHibernateBugWorkaround(query.Froms[0], query.Conditions, a => a);
		}

		public virtual void AddOrdering(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			QueryBuilderHelpers.AddOrderingToQuery(HqlConstants.WorklistItemQualifier, query, args.Criteria, RemapHqlExpression);
		}

		public virtual void AddCountProjection(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Selects.AddRange(HqlConstants.DefaultCountProjection);
		}

		public virtual void AddItemProjection(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Selects.AddRange(GetWorklistItemSelects(args.Projection));
		}

		public virtual void AddPagingRestriction(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Page = args.Page;
		}

		public object[] PreProcessTuple(object[] tuple, QueryBuilderArgs args)
		{
			// assume we will return a tuple of the same dimension as the input
			// therefore, we just map field for field, calling to PreProcessTupleField on each field
			var projection = args.Projection;
			for (var i = 0; i < projection.Fields.Count; i++)
			{
				tuple[i] = PreProcessTupleField(tuple[i], projection.Fields[i]);
			}
			return tuple;
		}

		#endregion

		protected virtual object PreProcessTupleField(object value, WorklistItemField field)
		{
			// special handling of this field, because Name is not a persistent property
			if (field == WorklistItemField.ProcedureStepName)
				return ((ProcedureStep) value).Name;

			// convert entities to entity refs
			if (field.IsEntityRefField)
				return GetEntityRef(value);

			// no special processing required
			return value;
		}

		protected static string RemapHqlExpression(string hqlExpression)
		{
			foreach (var kvp in HqlConstants.MapCriteriaKeyToHql)
			{
				var target = string.Format("{0}.{1}.", HqlConstants.WorklistItemQualifier, kvp.Key);
				var hqlReplacement = string.Format("{0}.", kvp.Value);
				if (hqlExpression.StartsWith(target))
					return hqlExpression.Replace(target, hqlReplacement);
			}
			return hqlExpression;
		}

		private static HqlSelect[] GetWorklistItemSelects(WorklistItemProjection projection)
		{
			return CollectionUtils.Map<WorklistItemField, HqlSelect>(projection.Fields, MapWorklistItemFieldToHqlSelect).ToArray();
		}

		/// <summary>
		/// Maps the <see cref="WorklistItemField"/> value to an <see cref="HqlSelect"/> object.
		/// </summary>
		private static HqlSelect MapWorklistItemFieldToHqlSelect(WorklistItemField itemField)
		{
			return HqlConstants.MapWorklistItemFieldToHqlSelect[itemField];
		}

		protected static EntityRef GetEntityRef(object entity)
		{
			return entity == null ? null : ((Entity) entity).GetRef();
		}
	}
}
