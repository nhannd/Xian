#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Owls
{
	using UpdateViewItemDelegate = UpdateViewItemDelegate<WorklistViewItemBase>;


	/// <summary>
	/// ProtocolWorklistViewItem entity
	/// </summary>
	public partial class ProtocolWorklistViewItem
	{
		private static readonly Dictionary<WorklistItemField, UpdateViewItemDelegate> _fieldMappings
			= new Dictionary<WorklistItemField, UpdateViewItemDelegate>();

		static ProtocolWorklistViewItem()
		{
			_fieldMappings.Add(WorklistItemField.Protocol,
				(item, value, updateReferences) => ((ProtocolWorklistViewItem)item).SetProtocolInfo((Protocol)value));
		}

		public virtual void SetProtocolInfo(Protocol pr)
		{
			_protocol = new WorklistViewItemProtocolInfo(
				pr,
				pr.Version,
				pr.Status,
				pr.Author,
				pr.Supervisor);
		}

		protected override UpdateViewItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			UpdateViewItemDelegate updater;
			return _fieldMappings.TryGetValue(field, out updater) ? updater : base.GetFieldUpdater(field);
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}