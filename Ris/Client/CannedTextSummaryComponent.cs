using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/Tools/Canned Text", "Launch")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class CannedTextTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public void Launch()
		{
			try
			{
				if (_shelf == null)
				{
					CannedTextSummaryComponent component = new CannedTextSummaryComponent();

					_shelf = ApplicationComponent.LaunchAsShelf(
						this.Context.DesktopWindow,
						component,
						SR.TitleCannedText, ShelfDisplayHint.DockFloat);

					_shelf.Closed += delegate { _shelf = null; };
				}
				else
				{
					_shelf.Activate();
				}
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="CannedTextSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextSummaryComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextSummaryComponentViewExtensionPoint))]
	public class CannedTextSummaryComponent : SummaryComponentBase<CannedTextSummary, CannedTextTable>
	{
		private EventHandler _copyCannedTextRequested;

		private Action _duplicateCannedTextAction;
		private Action _copyCannedTextAction;

		#region Presentation Model

		public string Group
		{
			get
			{
				if (this.SelectedItems.Count != 1)
					return string.Empty;

				CannedTextSummary item = this.SelectedItems[0];
				return item.IsPersonal ? SR.ColumnPersonal : item.StaffGroup.Name;
			}
		}

		public string Name
		{
			get
			{
				if (this.SelectedItems.Count != 1)
					return string.Empty;

				return this.SelectedItems[0].Name;
			}
		}

		public string Category
		{
			get
			{
				if (this.SelectedItems.Count != 1)
					return string.Empty;

				return this.SelectedItems[0].Path;
			}
		}

		public string Preview
		{
			get
			{
				if (this.SelectedItems.Count != 1)
					return string.Empty;

				return this.SelectedItems[0].Text;
			}
		}

		public event EventHandler CopyCannedTextRequested
		{
			add { _copyCannedTextRequested += value; }
			remove { _copyCannedTextRequested -= value; }
		}

		public void DuplicateAdd()
		{
			try
			{
				CannedTextSummary item = CollectionUtils.FirstElement(this.SelectedItems);
				IList<CannedTextSummary> addedItems = new List<CannedTextSummary>();
				CannedTextEditorComponent editor = new CannedTextEditorComponent(item.CannedTextRef, true);

				ApplicationComponentExitCode exitCode = LaunchAsDialog(
					this.Host.DesktopWindow, editor, SR.TitleDuplicateCannedText);
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					addedItems.Add(editor.UpdatedCannedTextSummary);
					this.Table.Items.AddRange(addedItems);
					this.SummarySelection = new Selection(addedItems);
				}
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void CopyCannedText()
		{
			EventsHelper.Fire(_copyCannedTextRequested, this, EventArgs.Empty);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to support deletion.
		/// </summary>
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			_duplicateCannedTextAction = model.AddAction("duplicateCannedText", SR.TitleDuplicate, "Icons.DuplicateToolSmall.png",
				SR.TitleDuplicate, DuplicateAdd);

			_copyCannedTextAction = model.AddAction("copyCannedText", SR.TitleCopy, "Icons.CopyToolSmall.png",
				SR.MessageCopyToClipboard, CopyCannedText);

			model.Edit.Enabled = false;
			model.Delete.Enabled = false;
			_duplicateCannedTextAction.Enabled = false;
			_copyCannedTextAction.Enabled = false;

		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<CannedTextSummary> ListItems(int firstItem, int maxItems)
		{
			ListCannedTextResponse listResponse = null;
			Platform.GetService<ICannedTextService>(
				delegate(ICannedTextService service)
				{
					listResponse = service.ListCannedText(new ListCannedTextRequest(new SearchResultPage(firstItem, maxItems)));
				});

			return listResponse.CannedTexts;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<CannedTextSummary> addedItems)
		{
			addedItems = new List<CannedTextSummary>();
			CannedTextEditorComponent editor = new CannedTextEditorComponent();
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddCannedText);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.UpdatedCannedTextSummary);
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
		protected override bool EditItems(IList<CannedTextSummary> items, out IList<CannedTextSummary> editedItems)
		{
			editedItems = new List<CannedTextSummary>();
			CannedTextSummary item = CollectionUtils.FirstElement(items);

			CannedTextEditorComponent editor = new CannedTextEditorComponent(item.CannedTextRef);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateCannedText);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.UpdatedCannedTextSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<CannedTextSummary> items)
		{
			List<EntityRef> cannedTextRefs = CollectionUtils.Map<CannedTextSummary, EntityRef>(items,
				delegate(CannedTextSummary c) { return c.CannedTextRef; });

			Platform.GetService<ICannedTextService>(
				delegate(ICannedTextService service)
					{
						DeleteCannedTextRequest request = new DeleteCannedTextRequest(cannedTextRefs);
						service.DeleteCannedText(request);
					});

			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(CannedTextSummary x, CannedTextSummary y)
		{
			return x.CannedTextRef.Equals(y.CannedTextRef, true);
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			if (this.SelectedItems.Count == 1)
			{
				CannedTextSummary selectedItem = this.SelectedItems[0];

				_duplicateCannedTextAction.Enabled = true;
				_copyCannedTextAction.Enabled = true;

				if (selectedItem.IsPersonal)
				{
					this.ActionModel.Add.Enabled = 
						this.ActionModel.Edit.Enabled = 
						this.ActionModel.Delete.Enabled = HasPersonalAdminAuthority;
				}
				else
				{
					this.ActionModel.Add.Enabled = 
						this.ActionModel.Add.Enabled = 
						this.ActionModel.Add.Enabled = HasGroupAdminAuthority;
				}
			}
			else
			{
				_duplicateCannedTextAction.Enabled = false;
				_copyCannedTextAction.Enabled = false;
			}

			NotifyAllPropertiesChanged();
		}

		#endregion

		private static bool HasPersonalAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.CannedText.Personal); }
		}

		private static bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.CannedText.Group); }
		}
	}
}
