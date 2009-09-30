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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PhoneNumbersSummaryComponent : SummaryComponentBase<TelephoneDetail, TelephoneNumberTable>
    {
        private IList<TelephoneDetail> _phoneNumberList;
        private readonly List<EnumValueInfo> _phoneTypeChoices;

        public PhoneNumbersSummaryComponent(List<EnumValueInfo> phoneTypeChoices)
			: base(false)
        {
            _phoneTypeChoices = phoneTypeChoices;
        }

        public IList<TelephoneDetail> Subject
        {
            get { return _phoneNumberList; }
            set { _phoneNumberList = value; }
        }

        #region Overrides

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
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.Enabled = true;
			model.Edit.Enabled = false;
			model.Delete.Enabled = false;
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<TelephoneDetail> ListItems(int firstItem, int maxItems)
		{
			return _phoneNumberList;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<TelephoneDetail> addedItems)
		{
			addedItems = new List<TelephoneDetail>();

			TelephoneDetail phoneNumber = new TelephoneDetail();
			phoneNumber.Type = _phoneTypeChoices[0];

			PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber, _phoneTypeChoices);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddPhoneNumber);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(phoneNumber);
				_phoneNumberList.Add(phoneNumber);
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
		protected override bool EditItems(IList<TelephoneDetail> items, out IList<TelephoneDetail> editedItems)
		{
			editedItems = new List<TelephoneDetail>();
			TelephoneDetail oldItem = CollectionUtils.FirstElement(items);
			TelephoneDetail newItem = (TelephoneDetail)oldItem.Clone();

			PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(newItem, _phoneTypeChoices);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdatePhoneNumber);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(newItem);

				// Since there is no way to use IsSameItem to identify the address before and after are the same
				// We must manually remove the old and add the new item
				this.Table.Items.Replace(
					delegate(TelephoneDetail x) { return IsSameItem(oldItem, x); },
					newItem);

				// Preserve the order of the items
				int index = _phoneNumberList.IndexOf(oldItem);
				_phoneNumberList.Insert(index, newItem);
				_phoneNumberList.Remove(oldItem);

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
		protected override bool DeleteItems(IList<TelephoneDetail> items, out IList<TelephoneDetail> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<TelephoneDetail>();

			foreach (TelephoneDetail item in items)
			{
				deletedItems.Add(item);
				_phoneNumberList.Remove(item);
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(TelephoneDetail x, TelephoneDetail y)
		{
			return Equals(x, y);
		}

		#endregion
	}
}
