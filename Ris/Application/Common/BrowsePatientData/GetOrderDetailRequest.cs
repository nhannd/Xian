#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class GetOrderDetailRequest : DataContractBase
	{
		public GetOrderDetailRequest(EntityRef orderRef,
			bool includeVisit,
			bool includeProcedures,
			bool includeAlerts,
			bool includeNotes,
			bool includeAttachments,
			bool includeResultRecipients)
		{
			this.OrderRef = orderRef;
			this.IncludeVisit = includeVisit;
			this.IncludeProcedures = includeProcedures;
			this.IncludeAlerts = includeAlerts;
			this.IncludeNotes = includeNotes;
			this.IncludeAttachments = includeAttachments;
			this.IncludeResultRecipients = includeResultRecipients;
		}

		public GetOrderDetailRequest()
		{
		}

		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// Include order alerts.
		/// </summary>
		[DataMember]
		public bool IncludeAlerts;

		/// <summary>
		/// Include visit information.
		/// </summary>
		[DataMember]
		public bool IncludeVisit;

		/// <summary>
		/// Include detailed procedure information.
		/// </summary>
		[DataMember]
		public bool IncludeProcedures;

		/// <summary>
		/// Include order notes.
		/// </summary>
		[DataMember]
		public bool IncludeNotes;

		/// <summary>
		/// Include order attachments.
		/// </summary>
		[DataMember]
		public bool IncludeAttachments;

		/// <summary>
		/// Include order result recipients.
		/// </summary>
		[DataMember]
		public bool IncludeResultRecipients;

		/// <summary>
		/// A list of filters that determine which categories of order notes are returned. Optional, defaults to all.
		/// Ignored if <see cref="IncludeNotes"/> is false.
		/// </summary>
		[DataMember]
		public List<string> NoteCategoriesFilter;

		/// <summary>
		/// Include order extended properties.
		/// </summary>
		[DataMember]
		public bool IncludeExtendedProperties;
	}
}
