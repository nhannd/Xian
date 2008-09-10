using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Admin
{
	public class ProcedureTypeTable : Table<ProcedureTypeSummary>
	{
		public ProcedureTypeTable()
		{
			this.Columns.Add(new TableColumn<ProcedureTypeSummary, string>(
				"ID",
				delegate(ProcedureTypeSummary procedureType) { return procedureType.Id; },
				0.2f));

			this.Columns.Add(new TableColumn<ProcedureTypeSummary, string>(
				"Name",
				delegate(ProcedureTypeSummary procedureType) { return procedureType.Name; },
				1.0f));
		}
	}
}
