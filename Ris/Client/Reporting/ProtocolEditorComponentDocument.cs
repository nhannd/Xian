using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Reporting.Folders;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Document container for <see cref="ProtocolEditorComponent"/>
    /// This particular document has the added responsibility of creating and opening a new <see cref="ProtocolEditorComponentDocument"/> instance when 
    /// user specifies - via <see cref="ProtocolEditorComponent"/> - that they would like to protocol the next worklist item
    /// </summary>
    class ProtocolEditorComponentDocument : Document
    {
        #region Private Members

        private readonly ReportingWorklistItem _item;
        private readonly IReportingWorkflowItemToolContext _context;
        private ProtocolEditorComponent _component;

        #endregion

        #region Constructor

        public ProtocolEditorComponentDocument(ReportingWorklistItem item, IReportingWorkflowItemToolContext context)
            : base(item.OrderRef, context.DesktopWindow)
        {
            if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            _item = item;
            _context = context;
        }

        #endregion

        #region Document overrides

        public override string GetTitle()
        {
            return string.Format("A# {0} - {1}, {2}", _item.AccessionNumber, _item.PatientName.FamilyName, _item.PatientName.GivenName);
        }

        public override IApplicationComponent GetComponent()
        {
            ProtocolEditorComponent component = new ProtocolEditorComponent(_item);

            component.ProtocolAccepted += ProtocolAccepted;
            component.ProtocolCancelled += ProtocolCancelled;
            component.ProtocolSaved += ProtocolSaved;
            component.ProtocolSuspended += ProtocolSuspended;
            component.ProtocolRejected += ProtocolRejected;

            _component = component;
            return _component;
        }

        /// <summary>
        /// Augments Open() behaviour from <see cref="Document"/> by ensuring the opened protocol was successfully claimed.  If it was unsuccessful, 
        /// the document is closed and the next worklist item is opened.
        /// </summary>
        public new void Open()
        {
            base.Open();
            if(_component.ClaimProtocolResult == ClaimProtocolResult.Failed)
            {
                ProtocolAlreadyClaimed();
            }
        }

        #endregion

        #region ProtocolEditorComponent event handlers

        /// <summary>
        /// Updates specified folder and "To Be Protocolled" folders, and creates a document for the next worklist item if required.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> delegate indicating which folder to update.  Only the first matching folder is updated.</param>
        private void WorklistItemChanged(Predicate<IFolder> predicate)
        {
            if (predicate != null)
            {
                IFolder folder = CollectionUtils.SelectFirst<IFolder>(_context.Folders, predicate);
                Refresh(folder);
            }

            if (_component.ProtocolNextItem)
            {
                // if the next worklist item is to be protocolled, do so only after the worklist is refreshed
                _context.SelectedFolder.RefreshFinish += SelectedFolder_RefreshFinish;
            }

            Refresh(_context.SelectedFolder);

            CloseAndUnsubscribe();
        }

        void SelectedFolder_RefreshFinish(object sender, EventArgs e)
        {
            OpenNewProtocolEditorDocumentForReportingWorklistItem(GetNextWorklistItem());
            _context.SelectedFolder.RefreshFinish -= SelectedFolder_RefreshFinish;
        }

        private void ProtocolAlreadyClaimed()
        {
            WorklistItemChanged(null);
        }

        private void ProtocolAccepted(object sender, EventArgs e)
        {
            WorklistItemChanged(delegate(IFolder f) { return f is CompletedProtocolFolder; });
        }

        private void ProtocolSaved(object sender, EventArgs e)
        {
            WorklistItemChanged(delegate(IFolder f) { return f is DraftProtocolFolder; });
        }

        private void ProtocolSuspended(object sender, EventArgs e)
        {
            WorklistItemChanged(delegate(IFolder f) { return f is SuspendedProtocolFolder; });
        }

        private void ProtocolRejected(object sender, EventArgs e)
        {
            WorklistItemChanged(delegate(IFolder f) { return f is RejectedProtocolFolder; });
        }

        private void ProtocolCancelled(object sender, EventArgs e)
        {
            Refresh(_context.SelectedFolder);
            CloseAndUnsubscribe();
        }

        private void CloseAndUnsubscribe()
        {
            _component.ProtocolAccepted -= ProtocolAccepted;
            _component.ProtocolCancelled -= ProtocolCancelled;
            _component.ProtocolSaved -= ProtocolSaved;
            _component.ProtocolSuspended -= ProtocolSuspended;
            _component.ProtocolRejected -= ProtocolRejected;
            this.Close();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Refresh count or contents of specified <see cref="IFolder"/>
        /// </summary>
        /// <param name="folder"></param>
        private void Refresh(IFolder folder)
        {
            if (folder != null)
            {
                if (folder.IsOpen)
                    folder.Refresh();
                else
                    folder.RefreshCount();
            }
        }

        /// <summary>
        /// Open the specified <see cref="ReportingWorklistItem"/> in its own <see cref="ProtocolEditorComponentDocument"/>
        /// </summary>
        /// <param name="item"></param>
        private void OpenNewProtocolEditorDocumentForReportingWorklistItem(ReportingWorklistItem item)
        {
            if (item == null)
                return;

            Workspace workspace = DocumentManager.Get<ProtocolEditorComponentDocument>(item.OrderRef);
            if (workspace == null)
            {
                ProtocolEditorComponentDocument protocolEditorComponentDocument = new ProtocolEditorComponentDocument(item, _context);
                protocolEditorComponentDocument.Open();
            }
            else
            {
                workspace.Activate();
            }
        }

        /// <summary>
        /// Gets the next <see cref="ReportingWorklistItem"/> from this Document's context for
        /// </summary>
        /// <returns></returns>
        private ReportingWorklistItem GetNextWorklistItem()
        {
            // Selected folder should be "To Be Protocolled" folder
            IItemCollection items = _context.SelectedFolder.ItemsTable.Items;
            
            ReportingWorklistItem item = (ReportingWorklistItem)CollectionUtils.SelectFirst(
                items,
                delegate(object o) { return ((ReportingWorklistItem) o).ProcedureStepRef.Equals(_item.ProcedureStepRef) == false; });
            
            return item;
        }

        #endregion
    }
}
