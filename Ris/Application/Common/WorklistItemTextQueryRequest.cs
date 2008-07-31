using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorklistItemTextQueryRequest : TextQueryRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistItemTextQueryRequest()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		/// <param name="procedureStepClassName"></param>
		public WorklistItemTextQueryRequest(string textQuery, int specificityThreshold, string procedureStepClassName)
			: base(textQuery, specificityThreshold)
		{
			ProcedureStepClassName = procedureStepClassName;
		}

		/// <summary>
		/// Name of the procedure step class of interest.
		/// </summary>
		[DataMember]
		public string ProcedureStepClassName;

		/// <summary>
		/// Indicates whether the search is invoked from downtime recovery mode.
		/// </summary>
		[DataMember]
		public bool DowntimeRecoveryMode;
	}
}
