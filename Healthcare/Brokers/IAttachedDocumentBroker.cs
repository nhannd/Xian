using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IAttachedDocumentBroker
	{
		Patient FindPatientOwner(AttachedDocument document);
		Order FindOrderOwner(AttachedDocument document);
	}
}
