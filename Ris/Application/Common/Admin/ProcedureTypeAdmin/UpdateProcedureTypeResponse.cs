using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	[DataContract]
	public class UpdateProcedureTypeResponse : DataContractBase
	{
		public UpdateProcedureTypeResponse(ProcedureTypeSummary summary)
		{
			this.ProcedureType = summary;
		}

		[DataMember]
		public ProcedureTypeSummary ProcedureType;
	}
}
