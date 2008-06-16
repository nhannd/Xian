using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	[DataContract]
	public class LoadProcedureTypeForEditResponse : DataContractBase
	{
		public LoadProcedureTypeForEditResponse(ProcedureTypeDetail detail)
		{
			this.ProcedureType = detail;
		}

		[DataMember]
		public ProcedureTypeDetail ProcedureType;
	}
}
