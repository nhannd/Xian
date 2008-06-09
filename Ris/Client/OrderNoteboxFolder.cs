using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client
{
	public abstract class OrderNoteboxFolder : WorkflowFolder<OrderNoteboxItemSummary>
	{
		private readonly OrderNoteboxFolderSystem _folderSystem;
		private readonly string _noteboxClassName;

		public OrderNoteboxFolder(OrderNoteboxFolderSystem folderSystem, string folderDisplayName, string folderDescription, string noteboxClassName)
			: base(folderSystem, folderDisplayName, folderDescription, new OrderNoteboxTable())
		{
			_folderSystem = folderSystem;
			_noteboxClassName = noteboxClassName;

			this.RefreshTime = OrderNoteboxFolderSettings.Default.RefreshTime;
		}

		protected override bool CanQuery()
		{
			return true;
		}

		protected override QueryItemsResult QueryItems()
		{
			QueryItemsResult result = null;
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					QueryNoteboxRequest request = new QueryNoteboxRequest(_noteboxClassName, true, true);
					PrepareQueryRequest(request);
					QueryNoteboxResponse response = service.QueryNotebox(request);
					result = new QueryItemsResult(response.NoteboxItems, response.ItemCount);
				});

			return result;
		}

		protected override int QueryCount()
		{
			int count = -1;
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					QueryNoteboxRequest request = new QueryNoteboxRequest(_noteboxClassName, true, false);
					PrepareQueryRequest(request);
					QueryNoteboxResponse response = service.QueryNotebox(request);
					count = response.ItemCount;
				});

			return count;
		}

		protected virtual void PrepareQueryRequest(QueryNoteboxRequest request)
		{
			// nothing to do
		}

		public bool GetOperationEnablement(string operationName)
		{
			return _folderSystem.GetOperationEnablement(operationName);
		}
	}
}
