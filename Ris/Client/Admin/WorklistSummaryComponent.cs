#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[ExtensionPoint]
	public class WorklistSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistSummaryComponent class
	/// </summary>
	[AssociateView(typeof(WorklistSummaryComponentViewExtensionPoint))]
	public class WorklistSummaryComponent : SummaryComponentBase<WorklistAdminSummary, WorklistAdminSummaryTable, ListWorklistsRequest>
	{
		private readonly WorklistClassSummary _filterNone = new WorklistClassSummary(SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 SR.DummyItemNone,
																					 false,
																					 false);
		private readonly object _duplicateWorklistActionKey = new object();
		private string _name;
		private WorklistClassSummary _worklistClass;
		private ArrayList _worklistClassChoices = new ArrayList();
		private bool _includeUseDefinedWorklists;

		public override void Start()
		{
			Platform.GetService<IWorklistAdminService>(
					delegate(IWorklistAdminService service)
					{
						GetWorklistEditFormDataResponse response = service.GetWorklistEditFormData(new GetWorklistEditFormDataRequest());
						_worklistClassChoices.Add(_filterNone);
						response.WorklistClasses.Sort(
							delegate(WorklistClassSummary x, WorklistClassSummary y)
							{
								if (x.CategoryName.Equals(y.CategoryName))
									return x.DisplayName.CompareTo(y.DisplayName);
								else
									return x.CategoryName.CompareTo(y.CategoryName);
							});
						_worklistClassChoices.AddRange(response.WorklistClasses);
					});
			base.Start();
			_worklistClass = _filterNone;
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			// add a "duplicate worklist" action 
			this.ActionModel.AddAction(_duplicateWorklistActionKey, SR.TitleDuplicate, "Icons.DuplicateSmall.png", DuplicateWorklist);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Worklist);
			model[_duplicateWorklistActionKey].SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Worklist);
		}

		#region Presentation Model

		public object NullFilter
		{
			get { return _filterNone; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool IncludeUserDefinedWorklists
		{
			get { return _includeUseDefinedWorklists; }
			set { _includeUseDefinedWorklists = value; }
		}

		public object SelectedWorklistClass
		{
			get { return _worklistClass; }
			set
			{
				if (value == _filterNone)
					_worklistClass = null;
				else
					_worklistClass = (WorklistClassSummary)value;

				if (value == _filterNone)
					_worklistClass = _filterNone;
			}
		}

		public IList WorklistClassChoices
		{
			get { return _worklistClassChoices; }
		}

		public string FormatWorklistClassChoicesItem(object item)
		{
			if (item != _filterNone)
			{
				WorklistClassSummary summary = (WorklistClassSummary)item;
				return string.Format("{0} - {1}", summary.CategoryName, summary.DisplayName);
			}
			return SR.DummyItemNone;
		}

		public void DuplicateWorklist()
		{
			try
			{
				if (this.SelectedItems.Count != 1) return;

				WorklistAdminSummary worklist = CollectionUtils.FirstElement(this.SelectedItems);
				WorklistEditorComponent editor = new WorklistEditorComponent(worklist.WorklistRef, true, null, null);
				ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
					new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					this.Table.Items.AddRange(editor.EditedWorklistSummaries);
					this.SummarySelection = new Selection(editor.EditedWorklistSummaries);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();
			this.ActionModel[_duplicateWorklistActionKey].Enabled = this.SelectedItems.Count == 1;
		}

		protected override IList<WorklistAdminSummary> ListItems(ListWorklistsRequest request)
		{
			ListWorklistsResponse listResponse = null;

			Platform.GetService<IWorklistAdminService>(
				delegate(IWorklistAdminService service)
				{
					string[] classNames = (_worklistClass == null || _worklistClass == _filterNone) ?
						new string[] { } : new string[] { _worklistClass.ClassName };
					request.ClassNames = new List<string>(classNames);
					request.WorklistName = _name;
					request.IncludeUserDefinedWorklists = _includeUseDefinedWorklists;

					listResponse = service.ListWorklists(request);
				});

			return listResponse.WorklistSummaries;
		}

		protected override bool AddItems(out IList<WorklistAdminSummary> addedItems)
		{
			WorklistEditorComponent editor = new WorklistEditorComponent(true);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, SR.TitleAddWorklist, null, DialogSizeHint.Medium));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems = editor.EditedWorklistSummaries;
				return true;
			}
			else
			{
				addedItems = null;
				return false;
			}
		}

		protected override bool EditItems(IList<WorklistAdminSummary> items, out IList<WorklistAdminSummary> editedItems)
		{
			WorklistAdminSummary worklist = CollectionUtils.FirstElement(items);
			WorklistEditorComponent editor = new WorklistEditorComponent(worklist.WorklistRef, true);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, SR.TitleUpdateWorklist + " - " + worklist.DisplayName, null, DialogSizeHint.Medium));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems = editor.EditedWorklistSummaries;
				return true;
			}
			else
			{
				editedItems = null;
				return false;
			}
		}

		protected override bool DeleteItems(IList<WorklistAdminSummary> items, out IList<WorklistAdminSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<WorklistAdminSummary>();

			foreach (WorklistAdminSummary item in items)
			{
				try
				{
					Platform.GetService<IWorklistAdminService>(
						delegate(IWorklistAdminService service)
						{
							service.DeleteWorklist(new DeleteWorklistRequest(item.WorklistRef));
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

		protected override bool IsSameItem(WorklistAdminSummary x, WorklistAdminSummary y)
		{
			return x.WorklistRef.Equals(y.WorklistRef, true);
		}
	}
}
