#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Audit;
using DicomEventReceiptAction = ClearCanvas.Dicom.Audit.EventReceiptAction;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Represents the action taken by the application entity upon receiving a transfer of DICOM instances.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventReceiptAction"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventActionCode"/>,
	/// but <see cref="EventReceiptAction"/> uses <see cref="AuditHelper"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public class EventReceiptAction
	{
		/// <summary>
		/// The device does not already have these instances, and hence created new ones.
		/// </summary>
		public static DicomEventReceiptAction CreateNew
		{
			get { return DicomEventReceiptAction.CreateNew; }
		}

		/// <summary>
		/// The device already has these instances, has determined them to be no different from the arriving ones, and hence did not perform any action.
		/// </summary>
		public static DicomEventReceiptAction KeepExisting
		{
			get { return DicomEventReceiptAction.KeepExisting; }
		}

		/// <summary>
		/// The device already has these instances, has determined them to be different from the arriving ones, and hence updated the existing ones.
		/// </summary>
		public static DicomEventReceiptAction UpdateExisting
		{
			get { return DicomEventReceiptAction.UpdateExisting; }
		}

		/// <summary>
		/// The action that the receiving device took is unknown.
		/// </summary>
		public static DicomEventReceiptAction ActionUnknown
		{
			get { return DicomEventReceiptAction.ActionUnknown; }
		}

		private readonly DicomEventReceiptAction _action;

		private EventReceiptAction(DicomEventReceiptAction action)
		{
			_action = action;
		}

		public static implicit operator DicomEventReceiptAction(EventReceiptAction action)
		{
			return action._action;
		}

		public static implicit operator EventReceiptAction(DicomEventReceiptAction action)
		{
			return new EventReceiptAction(action);
		}
	}
}