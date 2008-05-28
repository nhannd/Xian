using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorklistTextQueryRequest : TextQueryRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistTextQueryRequest()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		/// <param name="procedureStepClassName"></param>
		public WorklistTextQueryRequest(string textQuery, int specificityThreshold, string procedureStepClassName)
			: base(textQuery, specificityThreshold)
		{
			ProcedureStepClassName = procedureStepClassName;
		}

		/// <summary>
		/// Name of the procedure step class of interest.
		/// </summary>
		[DataMember]
		public string ProcedureStepClassName;
	}
}
