using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client
{
	public interface  IOrderNoteboxItemToolContext : IWorkflowItemToolContext<OrderNoteboxItemSummary>
	{
		OrderNoteboxFolderSystemBase FolderSystem { get; }
	}

	public interface IOrderNoteboxFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public abstract class OrderNoteboxFolderSystemBase : WorkflowFolderSystem<OrderNoteboxItemSummary>
	{
		class OrderNoteboxItemToolContext : ToolContext, IOrderNoteboxItemToolContext
		{
			private readonly OrderNoteboxFolderSystemBase _owner;

			public OrderNoteboxItemToolContext(OrderNoteboxFolderSystemBase _owner)
			{
				this._owner = _owner;
			}

			#region IOrderNoteboxItemToolContext Members

			public OrderNoteboxFolderSystemBase FolderSystem
			{
				get { return _owner; }
			}

			#endregion

			#region IWorkflowItemToolContext<OrderNoteboxItemSummary> Members

			public ICollection<OrderNoteboxItemSummary> SelectedItems
			{
				get
				{
					return CollectionUtils.Map<object, OrderNoteboxItemSummary>(_owner.SelectedItems.Items,
						delegate(object item) { return (OrderNoteboxItemSummary)item; });
				}
			}

			#endregion

			#region IWorkflowItemToolContext Members

			public bool GetWorkflowOperationEnablement(string operationClass)
			{
				return _owner.GetOperationEnablement(operationClass);
			}

			public event EventHandler SelectionChanged
			{
				add { _owner.SelectedItemsChanged += value; }
				remove { _owner.SelectedItemsChanged -= value; }
			}

			public ISelection Selection
			{
				get { return _owner.SelectedItems; }
			}

			public IEnumerable Folders
			{
				get { return _owner.Folders; }
			}

			public IFolder SelectedFolder
			{
				get { return _owner.SelectedFolder; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.DesktopWindow; }
			}

			#endregion
		}

		class OrderNoteboxFolderToolContext : ToolContext, IOrderNoteboxFolderToolContext
		{
			private readonly OrderNoteboxFolderSystemBase _owner;

			public OrderNoteboxFolderToolContext(OrderNoteboxFolderSystemBase owner)
			{
				this._owner = owner;
			}

			#region IWorkflowFolderToolContext Members

			public IEnumerable Folders
			{
				get { return _owner.Folders; }
			}

			public IFolder SelectedFolder
			{
				get { return _owner.SelectedFolder; }
			}

			public event EventHandler SelectedFolderChanged
			{
				add { _owner.SelectedFolderChanged += value; }
				remove { _owner.SelectedFolderChanged -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.DesktopWindow; }
			}

			#endregion
		}

		#region private fields

		private IDictionary<string, bool> _workflowEnablement;
		private readonly IconSet _unacknowledgedNotesIconSet;
		private readonly string _baseTitle;

		#endregion

		public OrderNoteboxFolderSystemBase(
			string title,
			IFolderExplorerToolContext folderExplorer, 
			ExtensionPoint<IFolder> folderExtensionPoint,
			ExtensionPoint<ITool> itemToolExtensionPoint,
			ExtensionPoint<ITool> folderToolExtensionPoint)
			: base(title, folderExplorer, folderExtensionPoint)
		{
			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			_itemTools = new ToolSet(itemToolExtensionPoint, new OrderNoteboxItemToolContext(this));
			_folderTools = new ToolSet(folderToolExtensionPoint, new OrderNoteboxFolderToolContext(this));

			_unacknowledgedNotesIconSet = new IconSet("NoteUnread.png");
			_baseTitle = title;
		}

		public bool GetOperationEnablement(string operationName)
		{
			try
			{
				return _workflowEnablement == null ? false : _workflowEnablement[operationName];
			}
			catch (KeyNotFoundException)
			{
				Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
				return false;
			}
		}

		public override void SelectedItemDoubleClickedEventHandler(object sender, EventArgs e)
		{
			base.SelectedItemDoubleClickedEventHandler(sender, e);

			OrderNoteConversationTool notesTool =
				(OrderNoteConversationTool)CollectionUtils.SelectFirst(
					this.ItemTools.Tools,
					delegate(ITool tool) { return tool is OrderNoteConversationTool; });

			if (notesTool != null && notesTool.Enabled)
				notesTool.Open();
		}

		protected void OnPrimaryFolderCountChanged(object sender, System.EventArgs e)
		{
			IFolder folder = (IFolder)sender;
			this.Title = string.Format(SR.FormatOrderNoteboxFolderSystemTitle, _baseTitle, folder.TotalItemCount);
			this.TitleIcon = folder.TotalItemCount > 0 ? _unacknowledgedNotesIconSet : null;
		}
	}
}
