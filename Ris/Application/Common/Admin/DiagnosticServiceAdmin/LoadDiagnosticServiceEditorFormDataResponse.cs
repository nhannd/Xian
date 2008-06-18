using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class LoadDiagnosticServiceEditorFormDataResponse : DataContractBase
	{
		public LoadDiagnosticServiceEditorFormDataResponse(List<ProcedureTypeSummary> procedureTypeChoices)
		{
			this.ProcedureTypeChoices = procedureTypeChoices;
		}

		[DataMember]
		public List<ProcedureTypeSummary> ProcedureTypeChoices;
	}
}
