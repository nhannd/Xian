#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerReplaceDisabledContactPointsTableItem
	{
		private ExternalPractitionerContactPointDetail _selectedNewContactPoint;
		private event EventHandler<EventArgs> _selectedNewContactPointModified;

		public string AffectedOrdersCount { get; private set; }
		public ExternalPractitionerContactPointDetail OldContactPoint { get; private set; }
		public string OldContactPointInfo { get; private set; }
		public List<ExternalPractitionerContactPointDetail> NewContactPointChoices { get; private set; }
		public string NewContactPointInfo { get; private set; }

		public ExternalPractitionerReplaceDisabledContactPointsTableItem(ExternalPractitionerContactPointDetail oldContactPoint, List<ExternalPractitionerContactPointDetail> newContactPointChoices)
		{
			this.OldContactPoint = oldContactPoint;
			this.OldContactPointInfo = GetContactPointInfo(oldContactPoint);

			this.NewContactPointChoices = newContactPointChoices;

			this.AffectedOrdersCount = string.Format("{0} orders will be modified.", "???");
		}

		public void SetAffectedOrdersCount(int count)
		{
			this.AffectedOrdersCount = string.Format("{0} orders will be modified.", count);
		}

		public ExternalPractitionerContactPointDetail SelectedNewContactPoint
		{
			get { return _selectedNewContactPoint; }
			set
			{
				_selectedNewContactPoint = value;
				this.NewContactPointInfo = _selectedNewContactPoint == null ? null : GetContactPointInfo(_selectedNewContactPoint);
				EventsHelper.Fire(_selectedNewContactPointModified, this, EventArgs.Empty);
			}
		}

		public event EventHandler<EventArgs> SelectedNewContactPointModified
		{
			add { _selectedNewContactPointModified += value; }
			remove { _selectedNewContactPointModified += value; }
		}

		public string FormatNewContactPointChoice(object item)
		{
			var detail = (ExternalPractitionerContactPointDetail)item;
			
			return string.IsNullOrEmpty(detail.Description) 
				? ExternalPractitionerContactPointFormat.Format(detail, "%N")
				: ExternalPractitionerContactPointFormat.Format(detail, "%N - %D");
		}

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
