using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class AttachedDocumentBroker : IAttachedDocumentBroker
	{
		#region IAttachedDocumentBroker Members

		public Patient FindPatientOwner(AttachedDocument document)
		{
			var hqlFrom = new HqlFrom(typeof(Patient).Name, "p");
			hqlFrom.Joins.Add(new HqlJoin("p.Attachments", "pa"));

			var query = new HqlProjectionQuery(hqlFrom);

			var criteria = new PatientAttachmentSearchCriteria();
			criteria.Document.EqualTo(document);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("pa", criteria));

			var patients = ExecuteHql<Patient>(query);

			return CollectionUtils.FirstElement(patients);
		}

		public Order FindOrderOwner(AttachedDocument document)
		{
			var hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			hqlFrom.Joins.Add(new HqlJoin("o.Attachments", "oa"));

			var query = new HqlProjectionQuery(hqlFrom);

			var criteria = new OrderAttachmentSearchCriteria();
			criteria.Document.EqualTo(document);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("oa", criteria));

			var orders = ExecuteHql<Order>(query);

			return CollectionUtils.FirstElement(orders);
		}

		#endregion
	}
}
