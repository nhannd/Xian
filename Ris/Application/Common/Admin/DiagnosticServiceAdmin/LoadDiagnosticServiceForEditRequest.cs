using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class LoadDiagnosticServiceForEditRequest : DataContractBase
	{
		public LoadDiagnosticServiceForEditRequest(EntityRef entityRef)
		{
			this.DiagnosticServiceRef = entityRef;
		}

		[DataMember]
		public EntityRef DiagnosticServiceRef;
	}
}
