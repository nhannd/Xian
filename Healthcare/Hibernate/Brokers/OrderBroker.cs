using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IOrderBroker"/>. See OrderBroker.hbm.xml for queries.
	/// </summary>
	public partial class OrderBroker : IOrderBroker
	{
		#region IOrderBroker Members

		public Order FindDocumentOwner(AttachedDocument document)
		{
			var q = this.Context.GetNamedHqlQuery("documentOrderOwner");
			q.SetParameter(0, document);
			return (Order) q.UniqueResult(); 
		}

		#endregion
	}
}
