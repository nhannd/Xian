#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeNavigatorComponent : NavigatorComponentContainer
	{
		private ExternalPractitionerMergeSelectedDuplicateComponent _selectedDuplicateComponent;
		private ExternalPractitionerMergePropertiesComponent _mergePropertiesComponent;
		private ExternalPractitionerMergeSelectedContactPointsComponent _selectContactPointsComponent;
		private ExternalPractitionerMergeAffectedOrdersComponent _affectedOrdersComponent;
		private ExternalPractitionerOverviewComponent _confirmationComponent;

		private readonly EntityRef _originalPractitionerRef;
		private readonly EntityRef _specifiedDuplicatePractitionerRef;
		private readonly ExternalPractitionerDetail _mergedPractitioner;
		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerDetail _selectedDuplicate;

		/// <summary>
		/// Constructor for selecting a single practitioner to merge.
		/// </summary>
		public ExternalPractitionerMergeNavigatorComponent(EntityRef practitionerRef)
			: this(practitionerRef, null)
		{
		}

		/// <summary>
		/// Constructor for selecting two practitioners to merge.
		/// </summary>
		public ExternalPractitionerMergeNavigatorComponent(EntityRef practitionerRef, EntityRef duplicatePractitionerRef)
		{
			_originalPractitionerRef = practitionerRef;
			_specifiedDuplicatePractitionerRef = duplicatePractitionerRef;
			_mergedPractitioner = new ExternalPractitionerDetail();
		}

		public override void Start()
		{
			this.Pages.Add(new NavigatorPage(SR.TitleSelectDuplicate, _selectedDuplicateComponent = new ExternalPractitionerMergeSelectedDuplicateComponent(_specifiedDuplicatePractitionerRef)));
			this.Pages.Add(new NavigatorPage(SR.TitleResolvePropertyConflicts, _mergePropertiesComponent = new ExternalPractitionerMergePropertiesComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitleSelectActiveContactPoints, _selectContactPointsComponent = new ExternalPractitionerMergeSelectedContactPointsComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitleCorrectAffectedOrders, _affectedOrdersComponent = new ExternalPractitionerMergeAffectedOrdersComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitlePreviewMergedPractitioner, _confirmationComponent = new ExternalPractitionerOverviewComponent()));
			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_selectedDuplicateComponent.SelectedPractitionerChanged += delegate { this.ForwardEnabled = _selectedDuplicateComponent.HasValidationErrors == false; };
			_selectContactPointsComponent.ContactPointSelectionChanged += delegate { this.ForwardEnabled = _selectContactPointsComponent.HasValidationErrors == false; };

			base.Start();

			// Start the component with forward button disabled.
			// The button will be enabled if there is a practitioner selected.
			this.ForwardEnabled = false;

			// Immediately activate validation after component start
			this.ShowValidation(true);
		}

		public override bool ShowTree
		{
			// Disable tree pane, so user can only navigate with the Forward and Backward buttons.
			// It is very important that each page navigates forward in a sequential order.
			get { return false; }
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			if (DialogBoxAction.Cancel == this.Host.ShowMessageBox(SR.MessageConfirmMergePractitioners, MessageBoxActions.OkCancel))
				return;

			try
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new MergeExternalPractitionerRequest
						{
							MergedPractitioner = _mergedPractitioner,
							DuplicatePractitionerRef = _selectedDuplicate.PractitionerRef,
							ContactPointReplacements = _affectedOrdersComponent.ContactPointReplacementMap
						};

						service.MergeExternalPractitioner(request);
					});

				this.ExitCode = ApplicationComponentExitCode.Accepted;
				this.Host.Exit();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		protected override void MoveTo(int index)
		{
			var previousIndex = this.CurrentPageIndex;
			base.MoveTo(index);

			if (previousIndex < 0 || index > previousIndex)
				OnMovedForward();
			else
				OnMovedBackward();
		}

		private void OnMovedForward()
		{
			var currentComponent = this.CurrentPage.Component;
			if (currentComponent == _selectedDuplicateComponent)
			{
				_originalPractitioner = LoadPractitionerDetail(_originalPractitionerRef);
				_selectedDuplicateComponent.OriginalPractitioner = _originalPractitioner;
				_mergePropertiesComponent.OriginalPractitioner = _originalPractitioner;
				_selectContactPointsComponent.OriginalPractitioner = _originalPractitioner;
			}
			else if (currentComponent == _mergePropertiesComponent)
			{
				// If selection change, load detail of the selected duplicate practitioner.
				if (_selectedDuplicateComponent.SelectedPractitioner == null)
					_selectedDuplicate = null;
				else if (_selectedDuplicate == null)
					_selectedDuplicate = LoadPractitionerDetail(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef);
				else if (!_selectedDuplicate.PractitionerRef.Equals(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef, true))
					_selectedDuplicate = LoadPractitionerDetail(_selectedDuplicateComponent.SelectedPractitioner.PractitionerRef);

				_mergePropertiesComponent.DuplicatePractitioner = _selectedDuplicate;
			}
			else if (currentComponent == _selectContactPointsComponent)
			{
				_selectContactPointsComponent.DuplicatePractitioner = _selectedDuplicate;
			}
			else if (currentComponent == _affectedOrdersComponent)
			{
				_affectedOrdersComponent.ActiveContactPoints = _selectContactPointsComponent.ActiveContactPoints;
				_affectedOrdersComponent.DeactivatedContactPoints = _selectContactPointsComponent.DeactivatedContactPoints;
			}
			else if (currentComponent == _confirmationComponent)
			{
				_mergedPractitioner.PractitionerRef = _originalPractitionerRef;
				_mergePropertiesComponent.Save(_mergedPractitioner);
				_selectContactPointsComponent.Save(_mergedPractitioner);
				_affectedOrdersComponent.Save();
				_confirmationComponent.PractitionerDetail = _mergedPractitioner;

				// The accept is enabled only on the very last page.
				this.AcceptEnabled = true;
			}
		}

		private void OnMovedBackward()
		{
			// The accept is enabled only on the very last page.  Nothing else to do when moving backward.
			this.AcceptEnabled = false;
		}

		private static ExternalPractitionerDetail LoadPractitionerDetail(EntityRef practitionerRef)
		{
			ExternalPractitionerDetail detail = null;

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadExternalPractitionerForEditRequest(practitionerRef);
						var response = service.LoadExternalPractitionerForEdit(request);
						detail = response.PractitionerDetail;
					});
			}

			return detail;
		}
	}
}
