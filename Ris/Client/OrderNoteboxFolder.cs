using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client
{
	public abstract class OrderNoteboxFolder : WorkflowFolder<OrderNoteboxItemSummary>
	{
		private readonly OrderNoteboxFolderSystemBase _folderSystem;
		private IconSet _closedIconSet;
		private IconSet _openIconSet;

		private readonly string _noteboxClassName;

		public OrderNoteboxFolder(OrderNoteboxFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, string noteboxClassName)
			: base(folderSystem, folderDisplayName, folderDescription, new OrderNoteboxTable())
		{
			_folderSystem = folderSystem;

			_closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
			_openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
			this.IconSet = _closedIconSet;
			this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);

			_noteboxClassName = noteboxClassName;

			this.RefreshTime = OrderNoteboxFolderSettings.Default.RefreshTime;
		}

		public IconSet ClosedIconSet
		{
			get { return _closedIconSet; }
			set { _closedIconSet = value; }
		}

		public IconSet OpenIconSet
		{
			get { return _openIconSet; }
			set { _openIconSet = value; }
		}

		public override void OpenFolder()
		{
			if (_openIconSet != null)
				this.IconSet = _openIconSet;

			base.OpenFolder();
		}

		public override void CloseFolder()
		{
			if (_closedIconSet != null)
				this.IconSet = _closedIconSet;

			base.CloseFolder();
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

					QueryNoteboxResponse response = service.QueryNotebox(request);
					count = response.ItemCount;
				});

			return count;
		}

		public bool GetOperationEnablement(string operationName)
		{
			return _folderSystem.GetOperationEnablement(operationName);
		}
	}
}
