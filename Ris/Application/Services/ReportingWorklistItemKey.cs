using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ReportingWorklistItemKey
	{
		private readonly EntityRef _procedureStepRef;
		private readonly EntityRef _procedureRef;

		public ReportingWorklistItemKey(EntityRef procedureStepRef, EntityRef procedureRef)
		{
			_procedureStepRef = procedureStepRef;
			_procedureRef = procedureRef;
		}

		public EntityRef ProcedureStepRef
		{
			get { return _procedureStepRef; }
		}

		public EntityRef ProcedureRef
		{
			get { return _procedureRef; }
		}
	}
}
