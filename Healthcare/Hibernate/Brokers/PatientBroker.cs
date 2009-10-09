using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IPatientBroker"/>. See PatientBroker.hbm.xml for queries.
	/// </summary>
	public partial class PatientBroker : IPatientBroker
	{
		#region IPatientBroker Members

		public Patient FindDocumentOwner(AttachedDocument document)
		{
			var q = this.Context.GetNamedHqlQuery("documentPatientOwner");
			q.SetParameter(0, document);
			return (Patient) q.UniqueResult();
		}

		#endregion
	}
}
