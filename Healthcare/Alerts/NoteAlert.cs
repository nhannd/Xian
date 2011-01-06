#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
	[ExtensionOf(typeof(PatientAlertExtensionPoint))]
	public class NoteAlert : PatientAlertBase
	{
		public override string Id
		{
			get { return "NoteAlert"; }
		}

		public override AlertNotification Test(Patient patient, IPersistenceContext context)
		{
			var reasons = new List<string>();
			foreach (var note in patient.Notes)
			{
				if (note.IsCurrent && note.Category.Severity == NoteSeverity.H)
				{
					if (!string.IsNullOrEmpty(note.Comment))
						reasons.Add(string.Format("{0}: {1}", note.Category.Name, note.Comment));
					else
						reasons.Add(note.Category.Name);
				}
			}

			return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
		}
	}
}
