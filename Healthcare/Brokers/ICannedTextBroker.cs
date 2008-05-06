using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Brokers
{
	public partial interface ICannedTextBroker
	{
		IList<CannedText> FindCannedTextForStaff(Staff staff);
	}
}
