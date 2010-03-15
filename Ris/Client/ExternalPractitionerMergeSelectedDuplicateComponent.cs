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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectedDuplicateComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectedDuplicateComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectedDuplicateComponent : ApplicationComponent
	{
		public class ExternalPractitionerTable : Table<ExternalPractitionerSummary>
		{
			public ExternalPractitionerTable()
			{
				this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>("Name",
					prac => prac.Name.ToString(),
					0.5f));

				this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>("License #",
					prac => prac.LicenseNumber,
					0.5f));

				this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>("Billing #",
					prac => prac.BillingNumber,
					0.5f));
			}
		}

		private readonly ExternalPractitionerTable _practitionerTable;
		private ExternalPractitionerSummary _selectedItem;
		private event EventHandler _summarySelectionChanged;

		public ExternalPractitionerMergeSelectedDuplicateComponent()
		{
			_practitionerTable = new ExternalPractitionerTable();
		}

		public ITable PractitionerTable
		{
			get { return _practitionerTable; }
		}

		public IList<ExternalPractitionerSummary> ExternalPractitioners
		{
			get { return _practitionerTable.Items; }
			set
			{
				_practitionerTable.Items.Clear();
				_practitionerTable.Items.AddRange(value);
			}
		}

		public ExternalPractitionerSummary SelectedPractitioner
		{
			get { return _selectedItem; }
		}

		public ISelection SummarySelection
		{
			get
			{
				return new Selection(_selectedItem);
			}
			set
			{
				var previousSelection = new Selection(_selectedItem);
				if (previousSelection.Equals(value)) 
					return;

				_selectedItem = (ExternalPractitionerSummary) value.Item;
				NotifyPropertyChanged("SummarySelection");
				EventsHelper.Fire(_summarySelectionChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SummarySelectionChanged
		{
			add { _summarySelectionChanged += value; }
			remove { _summarySelectionChanged -= value; }
		}
	}
}
