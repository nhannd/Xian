using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface IOrderBroker
	{
		Order FindDocumentOwner(AttachedDocument document);
	}
}
