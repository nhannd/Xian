using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class LoadDiagnosticServiceForEditResponse : DataContractBase
	{
		public LoadDiagnosticServiceForEditResponse(DiagnosticServiceDetail detail)
		{
			this.DiagnosticService = detail;
		}

		[DataMember]
		public DiagnosticServiceDetail DiagnosticService;
	}
}
