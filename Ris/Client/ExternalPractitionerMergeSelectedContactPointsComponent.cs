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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectedContactPointsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectedContactPointsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectedContactPointsComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectedContactPointsComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectedContactPointsComponent : ApplicationComponent
	{
		public class CheckableExternalPractitionerContactPointTable : Table<Checkable<ExternalPractitionerContactPointDetail>>
		{
			public CheckableExternalPractitionerContactPointTable()
			{
				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, bool>("Check",
					checkableItem => checkableItem.IsChecked,
					(checkableItem, value) => checkableItem.IsChecked = value,
					0.15f));

				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, string>("Name",
					checkableItem => checkableItem.Item.Name,
					0.5f));

				this.Columns.Add(new TableColumn<Checkable<ExternalPractitionerContactPointDetail>, string>("Description",
					checkableItem => checkableItem.Item.Description,
					0.5f));
			}

			public List<ExternalPractitionerContactPointDetail> CheckedItems
			{
				get
				{
					var checkedItems = new List<ExternalPractitionerContactPointDetail>();
					CollectionUtils.ForEach(this.Items, 
						delegate(Checkable<ExternalPractitionerContactPointDetail> checkableItem)
							{
								if (checkableItem.IsChecked)
									checkedItems.Add(checkableItem.Item);
							});

					return checkedItems;
				}
			}

			public void SetItems(List<ExternalPractitionerContactPointDetail> contactPoints)
			{
				this.Items.Clear();

				var checkableItems = CollectionUtils.Map<ExternalPractitionerContactPointDetail, Checkable<ExternalPractitionerContactPointDetail>>(contactPoints,
					item => new Checkable<ExternalPractitionerContactPointDetail>(item));

				this.Items.AddRange(checkableItems);
			}
		}

		private List<ExternalPractitionerContactPointDetail> _contactPoints;
		private readonly CheckableExternalPractitionerContactPointTable _checkableContactPointTable;

		public ExternalPractitionerMergeSelectedContactPointsComponent()
		{
			_checkableContactPointTable = new CheckableExternalPractitionerContactPointTable();
		}

		public ITable ContactPointTable
		{
			get { return _checkableContactPointTable; }
		}

		public List<ExternalPractitionerContactPointDetail> ContactPoints
		{
			get { return _contactPoints; }
			set
			{
				_contactPoints = value;
				_checkableContactPointTable.SetItems(_contactPoints);
			}
		}

		public List<ExternalPractitionerContactPointDetail> CheckedItem
		{
			get { return _checkableContactPointTable.CheckedItems; }
		}
	}
}
