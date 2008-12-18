using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines the schema of the report content JSML.  This data-contract is never sent to the server.
	/// It is purely a local-contract for internal use.
	/// </summary>
	[DataContract]
	class ReportContent : DataContractBase
	{
		public ReportContent()
		{
		}

		public ReportContent(string reportText)
		{
			this.ReportText = reportText;
		}

		/// <summary>
		/// The free-text component of the report.
		/// </summary>
		[DataMember]
		public string ReportText;

		public string ToJsml()
		{
			return JsmlSerializer.Serialize(this, "Report", false);
		}
	}
}
