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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PerformedProcedureDicomSeriesComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class PerformedProcedureDicomSeriesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

    public class DicomSeriesTable : Table<DicomSeriesDetail>
    {
        public DicomSeriesTable()
        {
			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>("Series Number",
                delegate(DicomSeriesDetail detail) { return detail.SeriesNumber; }, 
                0.5f));

            this.Columns.Add(new TableColumn<DicomSeriesDetail, string>("Description",
                delegate(DicomSeriesDetail detail) { return detail.SeriesDescription; }, 
                1.0f));

            this.Columns.Add(new TableColumn<DicomSeriesDetail, int>("Number of Images",
                delegate(DicomSeriesDetail detail) { return detail.NumberOfSeriesRelatedInstances; }, 
                0.5f));

			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>("Study Instance UID",
				delegate(DicomSeriesDetail detail) { return detail.StudyInstanceUID; },
				1.0f));

			this.Columns.Add(new TableColumn<DicomSeriesDetail, string>("Series Instance UID",
				delegate(DicomSeriesDetail detail) { return detail.SeriesInstanceUID; },
				1.0f));
		}
    }

	/// <summary>
	/// PerformedProcedureDicomSeriesComponent class.
	/// </summary>
	public class PerformedProcedureDicomSeriesComponent : SummaryComponentBase<DicomSeriesDetail, DicomSeriesTable>, IPerformedStepEditorPage
	{
		private readonly IPerformedStepEditorContext _context;

		public PerformedProcedureDicomSeriesComponent(IPerformedStepEditorContext context)
		{
			_context = context;
			_context.SelectedPerformedStepChanged += OnSelectedPerformedStepChanged;
		}

		private void OnSelectedPerformedStepChanged(object sender, System.EventArgs e)
		{
			this.Table.Items.Clear();
			this.Table.Items.AddRange(_context.SelectedPerformedStep.DicomSeries);
		}

		#region SummaryComponentBase Overrides

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to support deletion.
		/// </summary>
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected override bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<DicomSeriesDetail> ListItems(int firstItem, int maxItems)
		{
			// return empty list, the OnSelectedPerformedStepChanged takes care of populating table
			return new List<DicomSeriesDetail>();
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<DicomSeriesDetail> addedItems)
		{
			addedItems = new List<DicomSeriesDetail>();

			DicomSeriesDetail detail = new DicomSeriesDetail();

			// Keep looping until user enters an unique Dicom Series Number, or cancel the add operation
			DicomSeriesEditorComponent editor = new DicomSeriesEditorComponent(detail);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddDicomSeries);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(detail);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<DicomSeriesDetail> items, out IList<DicomSeriesDetail> editedItems)
		{
			editedItems = new List<DicomSeriesDetail>();
			DicomSeriesDetail detail = CollectionUtils.FirstElement(items);
			DicomSeriesDetail clone = (DicomSeriesDetail) detail.Clone();

			// Keep looping until user enters an unique Dicom Series Number, or cancel the edit operation
			DicomSeriesEditorComponent editor = new DicomSeriesEditorComponent(clone);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateDicomSeries);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(clone);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<DicomSeriesDetail> items, out IList<DicomSeriesDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<DicomSeriesDetail>();

			foreach (DicomSeriesDetail item in items)
			{
				deletedItems.Add(item);
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(DicomSeriesDetail x, DicomSeriesDetail y)
		{
			return x.DicomSeriesRef.Equals(y.DicomSeriesRef, true);
		}

		#endregion

		#region IPerformedStepEditorPage Members

		public void Save()
		{
			if(_context.SelectedPerformedStep != null)
			{
				_context.SelectedPerformedStep.DicomSeries = new List<DicomSeriesDetail>(this.Table.Items);
			}
		}

		public Path Path
		{
			get { return new Path("DICOM Series", new ResourceResolver(this.GetType().Assembly)); }
		}

		public IApplicationComponent GetComponent()
		{
			return this;
		}

		#endregion
	}
}
