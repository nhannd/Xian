using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
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
        private IList<TSummary> _selectedItems;
        private readonly TTable _summaryTable;

        private CrudActionModel _actionModel;
        private PagingController<TSummary> _pagingController;

    	private readonly bool _dialogMode;
    	private bool _supportModified;


		public SummaryComponentBase()
			: this(false)
		{
		}

		public SummaryComponentBase(bool dialogMode)
		{
			_dialogMode = dialogMode;
			_summaryTable = new TTable();
			_selectedItems = new List<TSummary>();
		}

        #region ApplicationComponent overrides

        public override void Start()
        {
            InitializeTable(_summaryTable);

            _actionModel = new CrudActionModel(true, this.SupportsEdit, this.SupportsDelete,
				new ResourceResolver(this.GetType(), true));

            _actionModel.Add.SetClickHandler(AddItems);

			if (SupportsEdit)
            _actionModel.Edit.SetClickHandler(EditSelectedItems);

            if(SupportsDelete)
                _actionModel.Delete.SetClickHandler(DeleteSelectedItems);

			InitializeActionModel(_actionModel);

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
				_summaryTable.Items.AddRange(ListAllItems());
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
                }
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
					if (_supportModified)
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
					if (_supportModified)
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

                    if(DeleteItems(_selectedItems))
                    {
                        // remove from table
                        CollectionUtils.ForEach(_selectedItems, delegate(TSummary item) { _summaryTable.Items.Remove(item); });

                        // clear selection
                        this.SummarySelection = Selection.Empty;
						if (_supportModified)
							this.Modified = true;
					}
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
			else
				EditSelectedItems();
		}

		public override void Accept()
		{
			this.ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		public override void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

        #endregion

        #region Abstract/overridable members
		
        /// <summary>
        /// Gets the list of items to show in the table, according to the specifed first and max items.
        /// </summary>
        /// <param name="firstItem"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        protected abstract IList<TSummary> ListItems(int firstItem, int maxItems);

		/// <summary>
		/// Gets the list of all items to show in the table.
		/// </summary>
		/// <returns></returns>
		protected virtual IList<TSummary> ListAllItems()
		{
			throw new NotImplementedException();
		}

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
        /// Called to handle the "delete" action, if supported.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>True if items were deleted, false otherwise.</returns>
        protected abstract bool DeleteItems(IList<TSummary> items);

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
		protected virtual void InitializeActionModel(CrudActionModel model)
		{
			
		}

        /// <summary>
        /// Called when the user changes the selected items in the table.
        /// </summary>
        protected virtual void OnSelectedItemsChanged()
        {
			if (SupportsEdit)
				_actionModel.Edit.Enabled = _selectedItems.Count == 1;

            if(SupportsDelete)
                _actionModel.Delete.Enabled = _selectedItems.Count > 0;
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
		/// Also override ListAllItems if paging is not supported
		/// </summary>
		protected virtual bool SupportsPaging
    	{
			get { return true; }
    	}

        #endregion 
        
        /// <summary>
        /// Gets the action model.
        /// </summary>
        protected CrudActionModel ActionModel
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
		/// Gets or sets a value indicating whether this component will set this.Modified to true
		/// when the summary list changed.  This property is public because it is different per instance, not per class.
		/// </summary>
		public virtual bool SupportModified
		{
			get { return _supportModified; }
			set { _supportModified = value; }
		}

	}
}
