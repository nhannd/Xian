#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	public class ModalityTable : Table<ModalitySummary>
	{
		public ModalityTable()
		{
			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnID,
				modality => modality.Id, 0.2f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnName,
				modality => modality.Name, 1.0f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnAETitle,
				modality => modality.AETitle, 1.0f));

			this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnDicomModality,
				FormatDicomModality, 1.0f));
		}

		public string FormatDicomModality(ModalitySummary modality)
		{
			return modality.DicomModality == null ? null 
				: string.Format("{0} - {1}", modality.DicomModality.Value, modality.DicomModality.Description);
		}
	}
}
