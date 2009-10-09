using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IPatientBroker
	{
		Patient FindDocumentOwner(AttachedDocument document);
	}
}
