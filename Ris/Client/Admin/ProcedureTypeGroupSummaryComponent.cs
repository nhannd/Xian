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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;
using System.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProcedureTypeGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProcedureTypeGroupSummaryComponent class
    /// </summary>
	[AssociateView(typeof(ProcedureTypeGroupSummaryComponentViewExtensionPoint))]
	public class ProcedureTypeGroupSummaryComponent : SummaryComponentBase<ProcedureTypeGroupSummary, ProcedureTypeGroupSummaryTable>
    {
		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);
		private List<EnumValueInfo> _categoryChoices;
		private ArrayList _selectedCategories;
		
		private ListProcedureTypeGroupsRequest _listRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcedureTypeGroupSummaryComponent()
			: this(false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        public ProcedureTypeGroupSummaryComponent(bool dialogMode)
            :base(dialogMode)
        {
        }

		public override void Start()
		{
			Platform.GetService<IProcedureTypeGroupAdminService>(
				delegate(IProcedureTypeGroupAdminService service)
					{
						GetProcedureTypeGroupSummaryFormDataResponse response =
							service.GetProcedureTypeGroupSummaryFormData(new GetProcedureTypeGroupSummaryFormDataRequest());
						_categoryChoices = response.CategoryChoices;
					});

			_listRequest = new ListProcedureTypeGroupsRequest();

			base.Start();
		}
		#region Presentation Model

		public object NullFilterItem
		{
			get { return _nullFilterItem; }
		}

		public IList CategoryChoices
		{
			get { return _categoryChoices; }
		}

		public string FormatCategory(object item)
		{
			if (item is EnumValueInfo)
			{
				EnumValueInfo category = (EnumValueInfo)item;
				return category.Value;
			}
			else
				return item.ToString(); // place-holder items
		}

		public IList SelectedCategories
		{
			get { return _selectedCategories; }
			set
			{
				if (!CollectionUtils.Equal(value, _selectedCategories, false))
				{
					_selectedCategories = new ArrayList(value);
					Search();
				}
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureTypeGroup);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureTypeGroup);
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<ProcedureTypeGroupSummary> ListItems(int firstItem, int maxItems)
		{
			ListProcedureTypeGroupsResponse listResponse = null;
			Platform.GetService<IProcedureTypeGroupAdminService>(
				delegate(IProcedureTypeGroupAdminService service)
				{
					_listRequest.Page = new SearchResultPage(firstItem, maxItems);
					listResponse = service.ListProcedureTypeGroups(_listRequest);
				});

			return listResponse.Items;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProcedureTypeGroupSummary> addedItems)
		{
			addedItems = new List<ProcedureTypeGroupSummary>();
			ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent();
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProcedureTypeGroup);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProcedureTypeGroupSummary);
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
		protected override bool EditItems(IList<ProcedureTypeGroupSummary> items, out IList<ProcedureTypeGroupSummary> editedItems)
		{
			editedItems = new List<ProcedureTypeGroupSummary>();
			ProcedureTypeGroupSummary item = CollectionUtils.FirstElement(items);

			ProcedureTypeGroupEditorComponent editor = new ProcedureTypeGroupEditorComponent(item.ProcedureTypeGroupRef);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateProcedureTypeGroup);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProcedureTypeGroupSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ProcedureTypeGroupSummary> items)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(ProcedureTypeGroupSummary x, ProcedureTypeGroupSummary y)
		{
			return x.ProcedureTypeGroupRef.Equals(y.ProcedureTypeGroupRef, true);
		}

		#endregion

		private void Search()
		{
			try
			{
				_listRequest.CategoryFilter =
					CollectionUtils.Map<object, EnumValueInfo>(
						_selectedCategories,
						delegate(object o)
						{
							return (EnumValueInfo)o;
						});

				this.Table.Items.Clear();
				this.Table.Items.AddRange(this.PagingController.GetFirst());
			}
			catch (Exception e)
			{
				// search failed
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}
	}
}
