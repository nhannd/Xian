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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Desktop
{
    [ExtensionPoint]
    public class SummaryComponentBaseViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }


    /// <summary>
    /// Abstract base class for admin summary components.
    /// </summary>
    [AssociateView(typeof(SummaryComponentBaseViewExtensionPoint))]
    public abstract class SummaryComponentBase : ApplicationComponent
    {
        #region Presentation Model

        /// <summary>
        /// Gets the summary table action model.
        /// </summary>
        public abstract ActionModelNode SummaryTableActionModel { get; }

        /// <summary>
        /// Gets the summary table <see cref="ITable"/>.
        /// </summary>
		public abstract ITable SummaryTable { get; }

        /// <summary>
        /// Gets the summary table selection as an <cref="ISelection"/>.
        /// </summary>
        public abstract ISelection SummarySelection { get; set; }

		/// <summary>
		/// Handles the "search" action if supported.
		/// </summary>
		public abstract void Search();

        /// <summary>
        /// Handles the "add" action.
        /// </summary>
        public abstract void AddItems();

        /// <summary>
        /// Handles the "edit" action.
        /// </summary>
        public abstract void EditSelectedItems();

        /// <summary>
        /// Handles the "delete" action.
        /// </summary>
        public abstract void DeleteSelectedItems();

		/// <summary>
		/// Handles the "toggle activation" action.
		/// </summary>
		public abstract void ToggleSelectedItemsActivation();

		/// <summary>
		/// Gets a value indicating whether dialog accept/cancel buttons are visible.
		/// </summary>
		public abstract bool ShowAcceptCancelButtons { get; }

		/// <summary>
		/// Gets a value indicating whether accept button is enabled.
		/// </summary>
		public abstract bool AcceptEnabled { get; }

		/// <summary>
		/// Handles double-clicking on a list item.
		/// </summary>
    	public abstract void DoubleClickSelectedItem();

		/// <summary>
		/// Handles the accept button.
		/// </summary>
    	public abstract void Accept();

		/// <summary>
		/// Handles the cancel button.
		/// </summary>
    	public abstract void Cancel();

        #endregion
    }

    /// <summary>
    /// Abstract base class for admin summary components.
    /// </summary>
    /// <typeparam name="TSummary"></typeparam>
    /// <typeparam name="TTable"></typeparam>
    public abstract class SummaryComponentBase<TSummary, TTable> : SummaryComponentBase
        where TSummary : class
        where TTable : Table<TSummary>, new()
    {
		public class AdminActionModel : CrudActionModel
		{
			private static readonly object ToggleActivationKey = new object();

			public AdminActionModel(bool add, bool edit, bool delete, bool toggleActivation, IResourceResolver resolver)
				:base(add, edit, delete, resolver)
			{
				if (toggleActivation)
				{
					this.AddAction(ToggleActivationKey, "Toggle Activation", "Icons.ToggleActivationSmall.png");
				}
			}

			/// <summary>
			/// Gets the ToggleActivation action.
			/// </summary>
			public ClickAction ToggleActivation
			{
				get { return this[ToggleActivationKey]; }
			}
		}



        private IList<TSummary> _selectedItems;
        private readonly TTable _summaryTable;

		private AdminActionModel _actionModel;
        private PagingController<TSummary> _pagingController;

    	private readonly bool _dialogMode;
		private bool _setModifiedOnListChange;

    	private readonly bool _showActiveColumn;
		private event EventHandler _summarySelectionChanged;

		public SummaryComponentBase()
			: this(false)
		{
		}

		public SummaryComponentBase(bool dialogMode)
		{
			_dialogMode = dialogMode;
			_summaryTable = new TTable();
			_selectedItems = new List<TSummary>();

			// by default, we show the "active" column if not in dialog mode (e.g. running as an admin screen)
			// we could expose this for customization if exceptions to this rule are found
			_showActiveColumn = !_dialogMode;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this component will set <see cref="ApplicationComponent.Modified"/> to true
		/// when the summary list is changed.
		/// </summary>
		public bool SetModifiedOnListChange
		{
			get { return _setModifiedOnListChange; }
			set { _setModifiedOnListChange = value; }
		}

		#region ApplicationComponent overrides

        public override void Start()
        {
            InitializeTable(_summaryTable);

			// add the "Active" column if needed
			if (_showActiveColumn && SupportsDeactivation)
			{
				_summaryTable.Columns.Add(new TableColumn<TSummary, bool>("Active",
					delegate(TSummary item)
					{
						return !GetItemDeactivated(item);
					}, 0.15f));
			}

			// build the action model
			_actionModel = new AdminActionModel(
                this.SupportsAdd,
				this.SupportsEdit,
				this.SupportsDelete,
				this.SupportsDeactivation,
				new ResourceResolver(this.GetType(), true));

            if (SupportsAdd)
                _actionModel.Add.SetClickHandler(AddItems);

            if (SupportsEdit)
			{
				_actionModel.Edit.SetClickHandler(EditSelectedItems);
				_actionModel.Edit.Enabled = false;
			}

			if (SupportsDelete)
			{
				_actionModel.Delete.SetClickHandler(DeleteSelectedItems);
				_actionModel.Delete.Enabled = false;
			}

			if (SupportsDeactivation)
			{
				_actionModel.ToggleActivation.SetClickHandler(ToggleSelectedItemsActivation);
				_actionModel.ToggleActivation.Enabled = false;
			}

			InitializeActionModel(_actionModel);

			// setup paging
			if (SupportsPaging)
			{
				_pagingController = new PagingController<TSummary>(
					delegate(int firstRow, int maxRows)
					{
						return ListItems(firstRow, maxRows);
					}
				);

				_actionModel.Merge(new PagingActionModel<TSummary>(_pagingController, _summaryTable, Host.DesktopWindow));

				_summaryTable.Items.AddRange(_pagingController.GetFirst());
			}
			else
			{
				_summaryTable.Items.AddRange(ListItems(0, -1));
			}

            base.Start();
        }

        #endregion

        #region Presentation Model

        /// <summary>
        /// Gets the summary table action model.
        /// </summary>
        public override ActionModelNode SummaryTableActionModel
        {
            get { return _actionModel; }
        }

        /// <summary>
        /// Gets the summary table <see cref="ITable"/>.
        /// </summary>
        public override ITable SummaryTable
        {
            get { return _summaryTable; }
        }

        /// <summary>
        /// Gets the summary table selection as an <cref="ISelection"/>.
        /// </summary>
        public override ISelection SummarySelection
        {
            get
            {
                return new Selection(_selectedItems);
            }
            set
            {
                Selection previousSelection = new Selection(_selectedItems);
                if (!previousSelection.Equals(value))
                {
                    _selectedItems = new TypeSafeListWrapper<TSummary>(value.Items);
                    OnSelectedItemsChanged();
                    NotifyPropertyChanged("SummarySelection");
					EventsHelper.Fire(_summarySelectionChanged, this, EventArgs.Empty);
                }
            }
        }

		public event EventHandler SummarySelectionChanged
		{
			add { _summarySelectionChanged += value; }
			remove { _summarySelectionChanged -= value; }
		}

		public override void Search()
		{
			try
			{
				this.Table.Items.Clear();
				this.Table.Items.AddRange(this.PagingController.GetFirst());
			}
			catch (Exception e)
			{
				// search failed
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

        /// <summary>
        /// Handles the "add" action.
        /// </summary>
        public override void AddItems()
        {
            try
            {
                IList<TSummary> addedItems;
                if(AddItems(out addedItems))
                {
                    _summaryTable.Items.AddRange(addedItems);
                    this.SummarySelection = new Selection(addedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        /// <summary>
        /// Handles the "edit" action.
        /// </summary>
        public override void EditSelectedItems()
        {
            try
            {
                if (_selectedItems.Count != 1) return;

                IList<TSummary> editedItems;
                if (EditItems(_selectedItems, out editedItems))
                {
                    foreach (TSummary item in editedItems)
                    {
                        _summaryTable.Items.Replace(
                            delegate(TSummary x) { return IsSameItem(item, x); },
                            item);
                    }

                    this.SummarySelection = new Selection(editedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
				}
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        /// <summary>
        /// Handles the "delete" action.
        /// </summary>
        public override void DeleteSelectedItems()
        {
            try
            {
                if (_selectedItems.Count == 0) return;

                DialogBoxAction action = this.Host.ShowMessageBox(SR.MessageConfirmDeleteSelectedItems, MessageBoxActions.YesNo);
                if (action == DialogBoxAction.Yes)
                {
                	string failureMessage;
					IList<TSummary> deletedItems;

					if (DeleteItems(_selectedItems, out deletedItems, out failureMessage))
                    {
						List<TSummary> notDeletedItems = new List<TSummary>(_selectedItems);

                        // remove from table
						CollectionUtils.ForEach(deletedItems, 
							delegate(TSummary item)
								{
									notDeletedItems.Remove(item);
									_summaryTable.Items.Remove(item);
								});

                        // clear selection
						this.SummarySelection = new Selection(notDeletedItems);
						if (_setModifiedOnListChange)
							this.Modified = true;
					}

					if (!string.IsNullOrEmpty(failureMessage))
						this.Host.ShowMessageBox(failureMessage, MessageBoxActions.Ok);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

		public override void ToggleSelectedItemsActivation()
		{
            try
            {
                if (_selectedItems.Count == 0) return;

                IList<TSummary> editedItems;
                if (UpdateItemsActivation(_selectedItems, out editedItems))
                {
                    foreach (TSummary item in editedItems)
                    {
                        _summaryTable.Items.Replace(
                            delegate(TSummary x) { return IsSameItem(item, x); },
                            item);
                    }

                    this.SummarySelection = new Selection(editedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
				}
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
		}

		public override bool ShowAcceptCancelButtons
		{
			get { return _dialogMode; }
		}

		public override bool AcceptEnabled
		{
			get { return _selectedItems.Count > 0; }
		}

		public override void DoubleClickSelectedItem()
		{
			// double-click behaviour is different depending on whether we're running as a dialog box or not
			if (_dialogMode)
				Accept();
			else if (SupportsEdit)
				EditSelectedItems();
		}

		public override void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public override void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

        #endregion

        #region Abstract/overridable members
		
        /// <summary>
        /// Gets the list of items to show in the table, according to the specifed first and max items.
        /// If <see cref="SupportsPaging"/> is false, then this method should ignore the first and max items
        /// parameters and return all items.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<TSummary> ListItems(int firstRow, int maxRows);

        /// <summary>
        /// Called to handle the "add" action.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <returns>True if items were added, false otherwise.</returns>
        protected abstract bool AddItems(out IList<TSummary> addedItems);

        /// <summary>
        /// Called to handle the "edit" action, if supported
        /// </summary>
        /// <param name="items">A list of items to edit.</param>
        /// <param name="editedItems">The list of items that were edited.</param>
        /// <returns>True if items were edited, false otherwise.</returns>
        protected abstract bool EditItems(IList<TSummary> items, out IList<TSummary> editedItems);


		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected virtual bool UpdateItemsActivation(IList<TSummary> items, out IList<TSummary> editedItems)
		{
			editedItems = new List<TSummary>();
			return false;
		}

        /// <summary>
        /// Called to handle the "delete" action, if supported.
        /// </summary>
        /// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if any items were deleted, false otherwise.</returns>
		protected abstract bool DeleteItems(IList<TSummary> items, out IList<TSummary> deletedItems, out string failureMessage);

        /// <summary>
        /// Compares two items to see if they represent the same item.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected abstract bool IsSameItem(TSummary x, TSummary y);

        /// <summary>
        /// Override this method to perform custom initialization of the table,
        /// such as adding columns or setting other properties that control the table behaviour.
        /// </summary>
        /// <param name="table"></param>
        protected virtual void InitializeTable(TTable table)
        {

        }

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected virtual void InitializeActionModel(AdminActionModel model)
		{
		}

        /// <summary>
        /// Called when the user changes the selected items in the table.
        /// </summary>
        protected virtual void OnSelectedItemsChanged()
        {
			if(SupportsEdit)
				_actionModel.Edit.Enabled = _selectedItems.Count == 1;

            if(SupportsDelete)
                _actionModel.Delete.Enabled = _selectedItems.Count > 0;

			if (SupportsDeactivation)
				_actionModel.ToggleActivation.Enabled = _selectedItems.Count > 0;
        }

        /// <summary>
        /// Gets a value indicating whether this component supports add.  The default is true.
        /// Override this method to change support for edit.
        /// </summary>
        protected virtual bool SupportsAdd
        {
            get { return true; }
        }

		/// <summary>
		/// Gets a value indicating whether this component supports edit.  The default is true.
		/// Override this method to change support for edit.
		/// </summary>
		protected virtual bool SupportsEdit
		{
			get { return true; }
		}

        /// <summary>
        /// Gets a value indicating whether this component supports deletion.  The default is false.
        /// Override this method to change support for deletion.
        /// </summary>
        protected virtual bool SupportsDelete
        {
            get { return false; }
        }

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected virtual bool SupportsPaging
    	{
			get { return true; }
    	}

		/// <summary>
		/// Gets a value indicating whether the items listed by this component support de-activation.
		/// The default implementation looks for a boolean "Deactivated" field on the summary item class,
		/// and assumes true if this field is present.
		/// </summary>
		protected virtual bool SupportsDeactivation
		{
			get
			{
				FieldInfo f = GetDeactivatedField();
				return f != null && f.FieldType.Equals(typeof (bool));
			}
		}

		/// <summary>
		/// Called to determine whether the specified item is deactivated, assuming <see cref="SupportsDeactivation"/> is true.
		/// The default implementation queries the "Deactivated" field on the summary item class.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool GetItemDeactivated(TSummary item)
		{
			FieldInfo field = GetDeactivatedField();
			return field == null ? false : (bool)field.GetValue(item);
		}

		#endregion 
        
        /// <summary>
        /// Gets the action model.
        /// </summary>
        protected AdminActionModel ActionModel
        {
            get { return _actionModel; }
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        protected TTable Table
        {
            get { return _summaryTable; }
        }

		/// <summary>
		/// Gets the paging controller.
		/// </summary>
    	protected IPagingController<TSummary> PagingController
    	{
			get { return _pagingController; }
    	}

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        protected IList<TSummary> SelectedItems
        {
            get { return _selectedItems; }
        }

		/// <summary>
		/// Gets a value indicating whether the component is running in dialog mode.
		/// </summary>
    	protected bool DialogMode
    	{
			get { return _dialogMode; }
    	}

		private static FieldInfo GetDeactivatedField()
		{
			return typeof(TSummary).GetField("Deactivated");
		}

	}

	public abstract class SummaryComponentBase<TSummary, TTable, TListRequest> : SummaryComponentBase<TSummary, TTable>
		where TSummary : class
		where TTable : Table<TSummary>, new()
		where TListRequest : ListRequestBase, new()
	{
		protected SummaryComponentBase()
		{
		}

		protected SummaryComponentBase(bool dialogMode)
			:base(dialogMode)
		{
		}

		protected override IList<TSummary> ListItems(int firstRow, int maxRows)
		{
			TListRequest request = new TListRequest();
			request.Page.FirstRow = firstRow;
			request.Page.MaxRows = maxRows;
			request.IncludeDeactivated = !this.DialogMode;	// generally, include de-activated for admin scenario, but not dialog mode

			return ListItems(request);
		}

		protected abstract IList<TSummary> ListItems(TListRequest request);
	}

}
