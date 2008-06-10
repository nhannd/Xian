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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Configure", "Apply")]
	[ExtensionOf(typeof(OrderNoteboxFolderToolExtensionPoint))]
	public class OrderNoteboxConfigurationTool : Tool<IOrderNoteboxFolderToolContext>
	{
		public void Apply()
		{
			OrderNoteboxConfigurationComponent component = new OrderNoteboxConfigurationComponent();
			ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow, component, "Notebox Groups Configuration");
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="OrderNoteboxConfigurationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class OrderNoteboxConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// OrderNoteboxConfigurationComponent class
	/// </summary>
	[AssociateView(typeof(OrderNoteboxConfigurationComponentViewExtensionPoint))]
	public class OrderNoteboxConfigurationComponent : ApplicationComponent
	{
		class TableItem : Checkable<StaffGroupSummary>
		{
			private readonly bool _isNew;
			public TableItem(StaffGroupSummary group, bool isChecked, bool isNew)
				:base(group, isChecked)
			{
				_isNew = isNew;
			}

			public bool IsNew { get { return _isNew; } }
		}

		private Table<TableItem> _staffGroupTable;
		private TableItem _selectedTableItem;

		private StaffGroupLookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _staffGroupToAdd;
		private CrudActionModel _actionModel;

		/// <summary>
		/// Constructor
		/// </summary>
		public OrderNoteboxConfigurationComponent()
		{
		}

		public override void Start()
		{
			// create table
			_staffGroupTable = new Table<TableItem>();
			_staffGroupTable.Columns.Add(new TableColumn<TableItem, bool>("Show",
				delegate(TableItem item) { return item.IsChecked; },
				delegate(TableItem item, bool value) { item.IsChecked = value; }));

			_staffGroupTable.Columns.Add(new TableColumn<TableItem, string>("Name",
				delegate(TableItem item) { return item.Item.Name; }));
			_staffGroupTable.Columns.Add(new TableColumn<TableItem, string>("Description",
				delegate(TableItem item) { return item.Item.Description; }));

			_actionModel = new CrudActionModel(false, false, true);
			_actionModel.Delete.SetClickHandler(DeleteStaffGroup);
			_actionModel.Delete.Enabled = false;

			// load existing group memberships
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					ListStaffGroupsResponse response = service.ListStaffGroups(new ListStaffGroupsRequest());
					_staffGroupTable.Items.AddRange(
						CollectionUtils.Map<StaffGroupSummary, TableItem>(response.StaffGroups,
							delegate (StaffGroupSummary g) { return new TableItem(g, true, false);}));
				});


			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#region Presentation Model

		public ILookupHandler StaffGroupLookupHandler
		{
			get { return _staffGroupLookupHandler; }
		}

		public StaffGroupSummary StaffGroupToAdd
		{
			get { return _staffGroupToAdd; }
			set
			{
				if(!Equals(value, _staffGroupToAdd))
				{
					_staffGroupToAdd = value;
					NotifyPropertyChanged("StaffGroupToAdd");
				}
			}
		}

		public ITable StaffGroupTable
		{
			get { return _staffGroupTable; }
		}

		public ActionModelNode ActionModel
		{
			get { return _actionModel; }
		}

		public ISelection SelectedTableItem
		{
			get { return new Selection(_selectedTableItem); }
			set
			{
				if(!Equals(new Selection(_selectedTableItem), value))
				{
					_selectedTableItem = (TableItem)value.Item;
					NotifyPropertyChanged("SelectedTableItem");
					UpdateActionModel();
				}
			}
		}

		public void AddStaffGroup()
		{
			if(_staffGroupToAdd == null || _staffGroupTable.Items.Contains(_staffGroupToAdd))
				return;

			_staffGroupTable.Items.Add(new TableItem(_staffGroupToAdd, true, true));
		}

		public void DeleteStaffGroup()
		{
			if(_selectedTableItem == null || !_selectedTableItem.IsNew)
				return;

			TableItem itemToDelete = _selectedTableItem;
			_selectedTableItem = null;
			NotifyPropertyChanged("SelectedTableItem");

			_staffGroupTable.Items.Remove(itemToDelete);
		}

		public void Accept()
		{
			
		}

		public void Cancel()
		{
			
		}

		#endregion

		private void UpdateActionModel()
		{
			_actionModel.Delete.Enabled = _selectedTableItem != null && _selectedTableItem.IsNew;
		}

	}
}
