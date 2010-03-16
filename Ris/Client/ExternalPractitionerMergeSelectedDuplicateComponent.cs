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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

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
		private class ExternalPractitionerTable : Table<ExternalPractitionerSummary>
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

		private readonly ExternalPractitionerTable _table;
		private ExternalPractitionerSummary _selectedItem;
		private EntityRef _originalPractitionerRef;

		public ExternalPractitionerMergeSelectedDuplicateComponent()
		{
			_table = new ExternalPractitionerTable();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(_selectedItem != null, "Must select at least one practitioner")));

			base.Start();
		}

		public EntityRef PractitionerRef
		{
			get { return _originalPractitionerRef; }
			set
			{
				if (value != null && _originalPractitionerRef != null && _originalPractitionerRef.Equals(value, true))
					return;

				if (Equals(_originalPractitionerRef, value))
					return;

				_originalPractitionerRef = value;

				var duplicates = LoadDuplicates(_originalPractitionerRef);
				_table.Items.Clear();
				_table.Items.AddRange(duplicates);
			}
		}
		public ExternalPractitionerSummary SelectedPractitioner
		{
			get { return _selectedItem; }
		}

		#region Presentation Models

		public ITable PractitionerTable
		{
			get { return _table; }
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
			}
		}

		#endregion

		private static List<ExternalPractitionerSummary> LoadDuplicates(EntityRef practitionerRef)
		{
			var duplicates = new List<ExternalPractitionerSummary>();

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest(practitionerRef) { IncludeDuplicates = true };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						duplicates = response.Duplicates;
					});
			}

			return duplicates;
		}
	}
}
