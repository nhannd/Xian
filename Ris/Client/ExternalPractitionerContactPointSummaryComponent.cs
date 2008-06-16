#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointTable : Table<ExternalPractitionerContactPointDetail>
	{
		private event EventHandler _defaultContactPointChanged;

		public ExternalPractitionerContactPointTable()
		{
			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Name",
				delegate(ExternalPractitionerContactPointDetail cp) { return cp.Name; },
				1.0f));

			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Description",
				delegate(ExternalPractitionerContactPointDetail cp) { return cp.Description; },
				1.0f));

			this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, bool>("Default",
				delegate(ExternalPractitionerContactPointDetail cp) { return cp.IsDefaultContactPoint; },
				delegate(ExternalPractitionerContactPointDetail cp, bool value)
				{
					MakeDefaultContactPoint(cp);
				},
				1.0f));
		}

		public event EventHandler DefaultContactPointChanged
		{
			add { _defaultContactPointChanged += value; }
			remove { _defaultContactPointChanged -= value; }
		}

		public void MakeDefaultContactPoint(ExternalPractitionerContactPointDetail cp)
		{
			foreach (ExternalPractitionerContactPointDetail item in this.Items)
			{
				item.IsDefaultContactPoint = (item == cp);
				this.Items.NotifyItemUpdated(item);
			}

			EventsHelper.Fire(_defaultContactPointChanged, this, EventArgs.Empty);
		}
	}

    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerContactPointSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerContactPointSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerContactPointSummaryComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerContactPointSummaryComponentViewExtensionPoint))]
	public class ExternalPractitionerContactPointSummaryComponent : SummaryComponentBase<ExternalPractitionerContactPointDetail, ExternalPractitionerContactPointTable>
    {
    	private readonly EntityRef _practitionerRef;
		private Action _mergeContactPointAction;

        private readonly IList<EnumValueInfo> _addressTypeChoices;
        private readonly IList<EnumValueInfo> _phoneTypeChoices;
        private readonly IList<EnumValueInfo> _resultCommunicationModeChoices;

        /// <summary>
        /// Constructor for editing. Set the <see cref="Subject"/> property before starting.
        /// </summary>
        public ExternalPractitionerContactPointSummaryComponent(
			EntityRef practitionerRef,
			IList<EnumValueInfo> addressTypeChoices, 
			IList<EnumValueInfo> phoneTypeChoices, 
			IList<EnumValueInfo> resultCommunicationModeChoices)
            :base(false)
        {
			_practitionerRef = practitionerRef;
            _addressTypeChoices = addressTypeChoices;
            _phoneTypeChoices = phoneTypeChoices;
            _resultCommunicationModeChoices = resultCommunicationModeChoices;
        }

        /// <summary>
        /// Constructor for read-only selection. Set the <see cref="Subject"/> property before starting.
        /// </summary>
		public ExternalPractitionerContactPointSummaryComponent(EntityRef practitionerRef)
            :base(true)
        {
			_practitionerRef = practitionerRef;
            _addressTypeChoices = new List<EnumValueInfo>();
            _phoneTypeChoices = new List<EnumValueInfo>();
            _resultCommunicationModeChoices = new List<EnumValueInfo>();
        }

		public override void Start()
		{
			ExternalPractitionerContactPointTable thisTable = (ExternalPractitionerContactPointTable) this.SummaryTable;
			thisTable.DefaultContactPointChanged += delegate
				{
					if (this.SupportModified)
						this.Modified = true;
				};

			base.Start();
		}
        public IItemCollection<ExternalPractitionerContactPointDetail> Subject
        {
            get { return this.Table.Items; }
        }

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			_mergeContactPointAction = model.AddAction("mergeContactPoint", SR.TitleMergePractitioner, "Icons.MergeToolSmall.png",
				SR.TitleMergePractitioner, MergeSelectedContactPoint);
			_mergeContactPointAction.Enabled = false;
		}

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
		protected override IList<ExternalPractitionerContactPointDetail> ListItems(int firstItem, int maxItems)
		{
			throw new NotImplementedException();
		}

		protected override IList<ExternalPractitionerContactPointDetail> ListAllItems()
		{
			// don't have to do anything, because the table is populcated by parent object
			return new List<ExternalPractitionerContactPointDetail>();
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ExternalPractitionerContactPointDetail> addedItems)
		{
			addedItems = new List<ExternalPractitionerContactPointDetail>();

			ExternalPractitionerContactPointDetail contactPoint = new ExternalPractitionerContactPointDetail();
			contactPoint.PreferredResultCommunicationMode = _resultCommunicationModeChoices.Count > 0 ? _resultCommunicationModeChoices[0] : null;

			ExternalPractitionerContactPointEditorComponent editor = new ExternalPractitionerContactPointEditorComponent(
				contactPoint, 
				_addressTypeChoices, 
				_phoneTypeChoices, 
				_resultCommunicationModeChoices);

			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddContactPoint);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(contactPoint);

				// if item was made default, then make sure no other items are also set as default
				if (contactPoint.IsDefaultContactPoint)
					this.Table.MakeDefaultContactPoint(contactPoint);

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
		protected override bool EditItems(IList<ExternalPractitionerContactPointDetail> items, out IList<ExternalPractitionerContactPointDetail> editedItems)
		{
			editedItems = new List<ExternalPractitionerContactPointDetail>();
			ExternalPractitionerContactPointDetail item = CollectionUtils.FirstElement(items);

			ExternalPractitionerContactPointDetail contactPoint = (ExternalPractitionerContactPointDetail)item.Clone();

			ExternalPractitionerContactPointEditorComponent editor = new ExternalPractitionerContactPointEditorComponent(
				contactPoint, 
				_addressTypeChoices, 
				_phoneTypeChoices, 
				_resultCommunicationModeChoices);

			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, 
				editor, 
				string.Format(SR.TitleUpdateContactPoint, contactPoint.Name));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(contactPoint);
				// if item was made default, then make sure no other items are also set as default
				if (contactPoint.IsDefaultContactPoint)
					this.Table.MakeDefaultContactPoint(contactPoint);

				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ExternalPractitionerContactPointDetail> items)
		{
			// TODO implement delete action, which should de-activate the contact point (can't delete it)
			throw new NotImplementedException();
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
		{
			return x.ContactPointRef.Equals(y.ContactPointRef, true);
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			_mergeContactPointAction.Enabled =
				(this.SelectedItems.Count == 1 ||
				 this.SelectedItems.Count == 2);
		}

		public void MergeSelectedContactPoint()
		{
			try
			{
				ExternalPractitionerContactPointDetail firstSelectedItem = this.SelectedItems.Count > 0 ? this.SelectedItems[0] : null;
				ExternalPractitionerContactPointDetail secondSelectedItem = this.SelectedItems.Count > 1 ? this.SelectedItems[1] : null;

				ExternalPractitionerContactPointMergeComponent mergeComponent = new ExternalPractitionerContactPointMergeComponent(
					_practitionerRef,
					this.Table.Items,
					firstSelectedItem,
					secondSelectedItem);

				ApplicationComponentExitCode exitCode = LaunchAsDialog(
					this.Host.DesktopWindow, 
					mergeComponent,
					SR.TitleMergeContactPoints);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					this.Table.Items.Remove(mergeComponent.SelectedDuplicate);
					if (this.SupportModified)
						this.Modified = true;
				}
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}
	}
}
