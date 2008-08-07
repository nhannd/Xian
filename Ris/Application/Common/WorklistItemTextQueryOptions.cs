using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Provides options that control the behaviour of worklist item text queries.
	/// </summary>
	[Flags]
	[Serializable]
	public enum WorklistItemTextQueryOptions
	{
		/// <summary>
		/// Specifies that the search query string should be applied against properties
		/// of the patient/order associated with the worklist item.
		/// </summary>
		PatientOrder = 0x001,

		/// <summary>
		/// Specifies that the search query string should be applied against properties
		/// of the scheduled or actual performing staff of the procedure step (assuming the worklist item is based on a procedure step).
		/// </summary>
		ProcedureStepStaff = 0x002,

		/// <summary>
		/// Find items associated with procedure in 'Downtime Recovery Mode', as opposed to live worklist items.
		/// </summary>
		DowntimeRecovery = 0x800
	}
}
