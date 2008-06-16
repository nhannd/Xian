using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class UpdateDiagnosticServiceResponse : DataContractBase
	{
		public UpdateDiagnosticServiceResponse(DiagnosticServiceSummary summary)
		{
			this.DiagnosticService = summary;
		}

		[DataMember]
		public DiagnosticServiceSummary DiagnosticService;
	}
}
