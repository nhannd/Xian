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
	[MenuAction("launch", "global-menus/MenuTools/Canned Text", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.CannedText.Personal)]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.CannedText.Group)]
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
	public class CannedTextSummaryComponent : SummaryComponentBase<CannedTextSummary, CannedTextTable, ListCannedTextRequest>
	{
		private CannedTextDetail _selectedCannedTextDetail;
		private EventHandler _copyCannedTextRequested;

		private Action _duplicateCannedTextAction;
		private Action _copyCannedTextToClipboardAction;

		#region Presentation Model

		public string GetFullCannedText()
		{
			if (this.SelectedItems.Count != 1)
				return string.Empty;

			// if the detail object is not null, it means the selection havn't changed
			// no need to hit the server, return the text now
			if (_selectedCannedTextDetail != null)
				return _selectedCannedTextDetail.Text;

			CannedTextSummary summary = CollectionUtils.FirstElement(this.SelectedItems);

			try
			{
				Platform.GetService<ICannedTextService>(
					delegate(ICannedTextService service)
					{
						LoadCannedTextForEditResponse response = service.LoadCannedTextForEdit(new LoadCannedTextForEditRequest(summary.CannedTextRef));
						_selectedCannedTextDetail = response.CannedTextDetail;
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			return _selectedCannedTextDetail.Text;
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
				CannedTextEditorComponent editor = new CannedTextEditorComponent(GetCategoryChoices(), item.CannedTextRef, true);

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

		protected override bool SupportsEdit
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to support deletion.
		/// </summary>
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected override bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			_duplicateCannedTextAction = model.AddAction("duplicateCannedText", SR.TitleDuplicate, "Icons.DuplicateSmall.png",
				SR.TitleDuplicate, DuplicateAdd);

			_copyCannedTextToClipboardAction = model.AddAction("copyCannedText", SR.TitleCopy, "Icons.CopyToClipboardToolSmall.png",
				SR.MessageCopyToClipboard, CopyCannedText);

			model.Edit.Enabled = false;
			model.Delete.Enabled = false;
			_duplicateCannedTextAction.Enabled = false;
			_copyCannedTextToClipboardAction.Enabled = false;

		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<CannedTextSummary> ListItems(ListCannedTextRequest request)
		{
			ListCannedTextResponse listResponse = null;
			Platform.GetService<ICannedTextService>(
				delegate(ICannedTextService service)
				{
					listResponse = service.ListCannedText(request);
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
			CannedTextEditorComponent editor = new CannedTextEditorComponent(GetCategoryChoices());
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

			CannedTextEditorComponent editor = new CannedTextEditorComponent(GetCategoryChoices(), item.CannedTextRef);
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
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<CannedTextSummary> items, out IList<CannedTextSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<CannedTextSummary>();

			foreach (CannedTextSummary item in items)
			{
				try
				{
					Platform.GetService<ICannedTextService>(
						delegate(ICannedTextService service)
						{
							service.DeleteCannedText(new DeleteCannedTextRequest(item.CannedTextRef));
						});

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
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

				_copyCannedTextToClipboardAction.Enabled = true;

				this.ActionModel.Add.Enabled = HasPersonalAdminAuthority || HasGroupAdminAuthority;
				this.ActionModel.Delete.Enabled =
					_duplicateCannedTextAction.Enabled =
						selectedItem.IsPersonal && HasPersonalAdminAuthority || 
						selectedItem.IsGroup && HasGroupAdminAuthority;
			}
			else
			{
				_duplicateCannedTextAction.Enabled = false;
				_copyCannedTextToClipboardAction.Enabled = false;
			}

			// The detail is only loaded whenever a copy/drag is performed
			// Set this to null, so the view doesn't get wrong text data.
			_selectedCannedTextDetail = null;

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

		private List<string> GetCategoryChoices()
		{
			List<string> categoryChoices = new List<string>();
			CollectionUtils.ForEach<CannedTextSummary>(this.SummaryTable.Items,
				delegate(CannedTextSummary c)
				{
					if (!categoryChoices.Contains(c.Category))
						categoryChoices.Add(c.Category);
				});

			categoryChoices.Sort();

			return categoryChoices;
		}
	}
}
