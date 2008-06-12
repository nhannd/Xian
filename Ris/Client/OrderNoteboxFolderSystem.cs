using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class OrderNoteboxFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IOrderNoteboxItemToolContext : IWorkflowItemToolContext<OrderNoteboxItemSummary>
	{
	}

	public interface IOrderNoteboxFolderToolContext : IWorkflowFolderToolContext
	{
	}

	[ExtensionOf(typeof(OrderNoteboxFolderToolExtensionPoint))]
	public class OrderNoteboxRefreshTool : RefreshTool<IOrderNoteboxFolderToolContext>
	{
	}



	public class OrderNoteboxFolderSystem: WorkflowFolderSystem<
		OrderNoteboxItemSummary,
		OrderNoteboxFolderExtensionPoint,
		OrderNoteboxFolderToolExtensionPoint,
		OrderNoteboxItemToolExtensionPoint>
	{
		class OrderNoteboxItemToolContext : WorkflowItemToolContext, IOrderNoteboxItemToolContext
		{
			public OrderNoteboxItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
			{
			}
		}

		class OrderNoteboxFolderToolContext : WorkflowFolderToolContext, IOrderNoteboxFolderToolContext
		{
			public OrderNoteboxFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
			{
			}
		}

		private readonly IconSet _unacknowledgedNotesIconSet;
		private readonly string _baseTitle;

		public OrderNoteboxFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitleOrderNoteboxFolderSystem, folderExplorer)
		{
			_unacknowledgedNotesIconSet = new IconSet("NoteUnread.png");
			_baseTitle = SR.TitleOrderNoteboxFolderSystem;

			PersonalInboxFolder inboxFolder = new PersonalInboxFolder(this);
			inboxFolder.TotalItemCountChanged += OnPrimaryFolderCountChanged;
			this.AddFolder(inboxFolder);

			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					List<EntityRef> visibleGroups = OrderNoteboxFolderSystemSettings.Default.GroupFolders.StaffGroupRefs;
					ListStaffGroupsResponse response = service.ListStaffGroups(new ListStaffGroupsRequest());
					foreach (StaffGroupSummary group in response.StaffGroups)
					{
						if(CollectionUtils.Contains(visibleGroups,
							delegate (EntityRef groupRef) { return groupRef.Equals(group.StaffGroupRef, true);}))
						{
							GroupInboxFolder groupFolder = new GroupInboxFolder(this, group);
							groupFolder.IsStatic = false;
							this.AddFolder(groupFolder);
						}
					}
				});

			this.AddFolder(new SentItemsFolder(this));
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new OrderNoteboxFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new OrderNoteboxItemToolContext(this);
		}

		public override string PreviewUrl
		{
			get { return WebResourcesSettings.Default.EmergencyPhysicianOrderNoteboxFolderSystemUrl; }
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

		protected override ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request)
		{
			return new ListWorklistsForUserResponse(new List<WorklistSummary>());
		}

		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			return new Dictionary<string, bool>();
		}

		protected void OnPrimaryFolderCountChanged(object sender, System.EventArgs e)
		{
			IFolder folder = (IFolder)sender;
			this.Title = string.Format(SR.FormatOrderNoteboxFolderSystemTitle, _baseTitle, folder.TotalItemCount);
			this.TitleIcon = folder.TotalItemCount > 0 ? _unacknowledgedNotesIconSet : null;
		}
	}
}
