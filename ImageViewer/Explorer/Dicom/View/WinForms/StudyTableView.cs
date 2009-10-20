#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public class StudyTableView : TableView
	{
		private Dictionary<string, int> _manualColumnWidths = null;
		private bool _isInternalColumnWidthChange = false;

		public StudyTableView() : base()
		{
			base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			base.DataGridView.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
		}

		/// <summary>
		/// This property is not applicable for this class.
		/// </summary>
		/// <remarks>
		/// Column sizing mode is always automatically controlled.
		/// </remarks>
		[Browsable(false)]
		public override DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
		{
			get { return DataGridViewAutoSizeColumnsMode.Fill; }
			set { }
		}

		public override ITable Table
		{
			get { return base.Table; }
			set
			{
				_isInternalColumnWidthChange = true;
				base.AutoSizeColumnsMode = (_manualColumnWidths == null) ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;
				base.SuspendLayout();
				try
				{
					base.Table = value;
					this.AdjustColumnWidths();
				}
				finally
				{
					base.ResumeLayout(true);
					base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
					_isInternalColumnWidthChange = false;
				}
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			_isInternalColumnWidthChange = true;
			base.AutoSizeColumnsMode = (_manualColumnWidths == null) ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;
			base.SuspendLayout();
			try
			{
				AdjustColumnWidths();
			}
			finally
			{
				base.ResumeLayout(true);
				base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
				_isInternalColumnWidthChange = false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			_isInternalColumnWidthChange = true;
			base.AutoSizeColumnsMode = (_manualColumnWidths == null) ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;
			base.SuspendLayout();
			try
			{
				AdjustColumnWidths();
			}
			finally
			{
				base.ResumeLayout(true);
				base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
				_isInternalColumnWidthChange = false;
			}
		}

		private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{
			if (!_isInternalColumnWidthChange)
			{
				if (_manualColumnWidths == null)
					_manualColumnWidths = new Dictionary<string, int>();
				foreach (DataGridViewColumn column in base.DataGridView.Columns)
					_manualColumnWidths[column.Name] = column.Width;
			}
		}

		private void AdjustColumnWidths()
		{
			if (_manualColumnWidths != null)
			{
				int totalColumnWidth = 0;
				foreach (DataGridViewColumn column in base.DataGridView.Columns)
					totalColumnWidth += column.Visible ? GetManualColumnWidth(column) : 0;

				float clientAreaWidth = base.DataGridView.ClientSize.Width;
				VScrollBar scrollBar = (VScrollBar) CollectionUtils.SelectFirst(base.DataGridView.Controls, c => c is VScrollBar);
				if (scrollBar != null && scrollBar.Visible)
					clientAreaWidth -= scrollBar.Width;

				float widthMultiplier = 1;

				if (totalColumnWidth < clientAreaWidth)
					widthMultiplier = (clientAreaWidth)/totalColumnWidth;

				foreach (DataGridViewColumn column in base.DataGridView.Columns)
					column.Width = (int) (GetManualColumnWidth(column)*widthMultiplier);
			}
		}

		private int GetManualColumnWidth(DataGridViewColumn column)
		{
			if (!_manualColumnWidths.ContainsKey(column.Name))
				return column.Width;
			return _manualColumnWidths[column.Name];
		}
	}
}