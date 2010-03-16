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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectDefaultContactPointComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectDefaultContactPointComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectDefaultContactPointComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectDefaultContactPointComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectDefaultContactPointComponent : ApplicationComponent
	{
		private class ExternalPractitionerContactPointsSingleCheckTable : Table<Checkable<ExternalPractitionerContactPointDetail>>
		{
			public ExternalPractitionerContactPointsSingleCheckTable()
			{
				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, bool>("Default",
					checkableItem => checkableItem.IsChecked,
					OnItemChecked,
					0.15f));

				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, string>("Name",
					checkableItem => checkableItem.Item.Name,
					0.5f));

				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, string>("Description",
					checkableItem => checkableItem.Item.Description,
					0.5f));
			}

			public ExternalPractitionerContactPointDetail CheckedItem
			{
				get
				{
					var checkedItem = CollectionUtils.SelectFirst(this.Items,
						checkableItem => checkableItem.IsChecked);

					return checkedItem == null ? null : checkedItem.Item;
				}
			}

			public void SetItems(List<ExternalPractitionerContactPointDetail> contactPoints)
			{
				this.Items.Clear();

				var checkableItems = CollectionUtils.Map<ExternalPractitionerContactPointDetail, Checkable<ExternalPractitionerContactPointDetail>>(contactPoints,
					item => new Checkable<ExternalPractitionerContactPointDetail>(item));

				this.Items.AddRange(checkableItems);
			}

			private void OnItemChecked(Checkable<ExternalPractitionerContactPointDetail> item, bool value)
			{
				// Uncheck every item
				foreach (var checkableItem in this.Items)
				{
					checkableItem.IsChecked = checkableItem == item ? value : false;
					this.Items.NotifyItemUpdated(checkableItem);
				}
			}
		}

		private readonly ExternalPractitionerContactPointsSingleCheckTable _table;

		public ExternalPractitionerMergeSelectDefaultContactPointComponent()
		{
			_table = new ExternalPractitionerContactPointsSingleCheckTable();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ContactPointTable",
				component => new ValidationResult(_table.CheckedItem != null, "Must have at least one default contact point")));

			base.Start();
		}

		public ExternalPractitionerContactPointDetail SelectedContactPoint
		{
			get { return _table.CheckedItem; }
		}

		public List<ExternalPractitionerContactPointDetail> ContactPoints
		{
			get
			{
				return CollectionUtils.Map(_table.Items,
					(Checkable<ExternalPractitionerContactPointDetail> item) => item.Item);
			}
			set
			{
				UpdateContactPointsTable(value);
			}
		}

		public void Save(ExternalPractitionerDetail practitioner)
		{
			var checkedContact = _table.CheckedItem;

			// Update IsDefaultContactPoint property of all contact points.
			foreach (var cp in practitioner.ContactPoints)
			{
				cp.IsDefaultContactPoint = cp.ContactPointRef.Equals(checkedContact.ContactPointRef, false);
			}
		}

		#region Presentation Models

		public ITable ContactPointTable
		{
			get { return _table; }
		}

		#endregion

		private void UpdateContactPointsTable(List<ExternalPractitionerContactPointDetail> contactPoints)
		{
			var previouslyChecked = _table.CheckedItem;

			_table.SetItems(contactPoints);

			if (previouslyChecked != null)
			{
				var itemToCheck = CollectionUtils.SelectFirst(_table.Items,
					item => Equals(previouslyChecked.ContactPointRef, item.Item.ContactPointRef));

				if (itemToCheck != null)
					itemToCheck.IsChecked = true;
			}
		}
	}
}
