#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeAffectedOrderTableItem
	{
		public delegate void ShowOrderPreviewDelegate(OrderDetail order);
		public delegate void ShowPractitionerPreviewDelegate(ExternalPractitionerSummary practitioner);

		private readonly ShowOrderPreviewDelegate _showOrderPreviewDelegate;
		private readonly ShowPractitionerPreviewDelegate _showPractitionerPreviewDelegate;

		public ExternalPractitionerMergeAffectedOrderTableItem(
			OrderDetail order, 
			ResultRecipientDetail recipient,
			ShowOrderPreviewDelegate showOrderDetailDelegate,
			ShowPractitionerPreviewDelegate showPractitionerDetailDelegate)
		{
			this.Order = order;
			this.Recipient = recipient;
			_showOrderPreviewDelegate = showOrderDetailDelegate;
			_showPractitionerPreviewDelegate = showPractitionerDetailDelegate;
		}

		public OrderDetail Order { get; private set; }

		public ResultRecipientDetail Recipient { get; private set; }

		public ExternalPractitionerContactPointDetail SelectedContactPoint { get; set; }

		public List<ExternalPractitionerContactPointDetail> ContactPointChoices { get; set; }

		#region Presentation Model

		public void ShowOrderPreview()
		{
			_showOrderPreviewDelegate(this.Order);
		}

		public void ShowPractitionerPreview()
		{
			_showPractitionerPreviewDelegate(this.Recipient.Practitioner);
		}

		public string OrderInfo
		{
			get { return string.Format("{0} - {1}", AccessionFormat.Format(this.Order.AccessionNumber), this.Order.DiagnosticService.Name); }
		}

		public string PractitionerInfo
		{
			get { return PersonNameFormat.Format(this.Recipient.Practitioner.Name); }
		}

		public string PractitionerRole
		{
			get
			{
				var isOrderingPhysician = this.Recipient.Practitioner.PractitionerRef.Equals(this.Order.OrderingPractitioner.PractitionerRef, false);
				return isOrderingPhysician ? SR.LabelOrderedBy : SR.LabelCopiesTo;
			}
		}

		public string OldContactPointInfo
		{
			get { return GetContactPointInfo(this.Recipient.ContactPoint); }
		}

		public string NewContactPointInfo
		{
			get { return this.SelectedContactPoint == null ? null : GetContactPointInfo(this.SelectedContactPoint); }
		}

		public string FormatItem(object item)
		{
			return ExternalPractitionerContactPointFormat.Format(item as ExternalPractitionerContactPointDetail, "%N - %D");
		}

		#endregion

		private static string GetContactPointInfo(ExternalPractitionerContactPointDetail cp)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Contact Point: {0}", cp.Name);
			builder.AppendLine();
			if (!string.IsNullOrEmpty(cp.Description))
			{
				builder.AppendFormat("Description: {0}", cp.Description);
				builder.AppendLine();
			}
			builder.AppendFormat(SR.FormatPhone, cp.CurrentPhoneNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatFax, cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatAddress, cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress));
			builder.AppendLine();
			builder.AppendFormat(SR.FormatEmail, cp.CurrentEmailAddress == null ? "" : cp.CurrentEmailAddress.Address);
			return builder.ToString();
		}
	}
}
