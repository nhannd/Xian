#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// VisitSummaryComponent class
    /// </summary>
    public class VisitSummaryComponent : SummaryComponentBase<VisitSummary, VisitSummaryTable>
    {
        private readonly EntityRef _patientRef;

        public VisitSummaryComponent(EntityRef patientRef, bool dialogMode)
			:base(dialogMode)
        {
            _patientRef = patientRef;
        }

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Visit.Create);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Visit.Update);
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<VisitSummary> ListItems(int firstItem, int maxItems)
		{
			ListVisitsForPatientResponse listResponse = null;
			Platform.GetService<IVisitAdminService>(
				delegate(IVisitAdminService service)
				{
					listResponse = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientRef));
				});

			return listResponse.Visits;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<VisitSummary> addedItems)
		{
			addedItems = new List<VisitSummary>();
			VisitEditorComponent editor = new VisitEditorComponent(_patientRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddVisit);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.VisitSummary);
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
		protected override bool EditItems(IList<VisitSummary> items, out IList<VisitSummary> editedItems)
		{
			editedItems = new List<VisitSummary>();
			VisitSummary item = CollectionUtils.FirstElement(items);

			VisitEditorComponent editor = new VisitEditorComponent(item);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateVisit);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.VisitSummary);
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
		protected override bool DeleteItems(IList<VisitSummary> items, out IList<VisitSummary> deletedItems, out string failureMessage)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(VisitSummary x, VisitSummary y)
		{
			return x.VisitRef.Equals(y.VisitRef, true);
		}
	}
}
