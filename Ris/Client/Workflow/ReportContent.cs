#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Utilities;
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
			return JsmlSerializer.Serialize(this, "Report");
		}
	}
}
