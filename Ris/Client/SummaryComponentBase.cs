using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Abstract base class for admin summary components.
    /// </summary>
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
        private TTable _summaryTable;

        private CrudActionModel _actionModel;
        private PagingController<TSummary> _pagingController;

        #region ApplicationComponent overrides

        public override void Start()
        {
            _summaryTable = new TTable();
            InitializeTable(_summaryTable);

            _selectedItems = new List<TSummary>();

            _actionModel = new CrudActionModel(true, true, this.SupportsDelete);
            _actionModel.Add.SetClickHandler(AddItems);
            _actionModel.Edit.SetClickHandler(EditSelectedItems);
            if(SupportsDelete)
                _actionModel.Delete.SetClickHandler(DeleteSelectedItems);

            _pagingController = new PagingController<TSummary>(
                delegate(int firstRow, int maxRows)
                {
                    return ListItems(firstRow, maxRows);
                }
            );

            _actionModel.Merge(new PagingActionModel<TSummary>(_pagingController, _summaryTable, Host.DesktopWindow));

            _summaryTable.Items.AddRange(_pagingController.GetFirst());

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
                    }
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
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
        /// Called to handle the "add" action.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <returns>True if items were added, false otherwise.</returns>
        protected abstract bool AddItems(out IList<TSummary> addedItems);

        /// <summary>
        /// Called to handle the "edit" action.
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
        /// Initializes the table.  Override this method to perform custom initialization of the table,
        /// such as adding columns or setting other properties that control the table behaviour.
        /// </summary>
        /// <param name="table"></param>
        protected virtual void InitializeTable(TTable table)
        {

        }

        /// <summary>
        /// Called when the user changes the selected items in the table.
        /// </summary>
        protected virtual void OnSelectedItemsChanged()
        {
            _actionModel.Edit.Enabled = _selectedItems.Count == 1;

            if(SupportsDelete)
                _actionModel.Delete.Enabled = _selectedItems.Count > 0;
        }

        /// <summary>
        /// Gets a value indicating whether this component supports deletion.  The default is false.
        /// Override this method to support deletion.
        /// </summary>
        protected virtual bool SupportsDelete
        {
            get { return false; }
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
        /// Gets the selected items.
        /// </summary>
        protected IList<TSummary> SelectedItems
        {
            get { return _selectedItems; }
        }
    }
}
