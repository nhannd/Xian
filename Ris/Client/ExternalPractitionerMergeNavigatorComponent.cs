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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeNavigatorComponent : NavigatorComponentContainer
	{
		private ExternalPractitionerMergeSelectedDuplicateComponent _selectedDuplicateComponent;
		private ExternalPractitionerMergePropertiesComponent _mergePropertiesComponent;
		private ExternalPractitionerSelectDisabledContactPointsComponent _selectContactPointsComponent;
		private ExternalPractitionerReplaceDisabledContactPointsComponent _replaceContactPointsComponent;
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
			this.Pages.Add(new NavigatorPage(SR.TitleSelectActiveContactPoints, _selectContactPointsComponent = new ExternalPractitionerSelectDisabledContactPointsComponent()));
			this.Pages.Add(new NavigatorPage(SR.TitleReplaceInactiveContactPoints, _replaceContactPointsComponent = new ExternalPractitionerReplaceDisabledContactPointsComponent()));
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
				var defaultContactPoint = CollectionUtils.SelectFirst(_mergedPractitioner.ContactPoints, cp => cp.IsDefaultContactPoint);
				var deactivatedContactPoints = CollectionUtils.Select(_mergedPractitioner.ContactPoints, cp => cp.Deactivated);

				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new MergeExternalPractitionerRequest
						{
							RightPractitionerRef = _mergedPractitioner.PractitionerRef,
							LeftPractitionerRef = _selectedDuplicate.PractitionerRef,
							Name = _mergedPractitioner.Name,
							LicenseNumber = _mergedPractitioner.LicenseNumber,
							BillingNumber = _mergedPractitioner.BillingNumber,
							ExtendedProperties = _mergedPractitioner.ExtendedProperties,
							DefaultContactPointRef = defaultContactPoint == null ? null : defaultContactPoint.ContactPointRef,
							DeactivatedContactPointRefs = CollectionUtils.Map(deactivatedContactPoints, (ExternalPractitionerContactPointDetail cp) => cp.ContactPointRef),
							ContactPointReplacements = _replaceContactPointsComponent.ContactPointReplacements
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

			if (CanMoveTo(index) == false)
				return;

			base.MoveTo(index);

			if (previousIndex < 0 || index > previousIndex)
				OnMovedForward();
			else
				OnMovedBackward();
		}

		private bool CanMoveTo(int index)
		{
			// don't prevent moving to first page during initialization
			if (this.CurrentPageIndex < 0)
				return true;

			// Moving forward
			if (index > this.CurrentPageIndex)
				if(this.CurrentPage.Component == _replaceContactPointsComponent && _replaceContactPointsComponent.HasValidationErrors)
				{
					_replaceContactPointsComponent.ShowValidation(true);
					return false;
				}

			// Moving back
			return true;
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
			else if (currentComponent == _replaceContactPointsComponent)
			{
				_replaceContactPointsComponent.ActiveContactPoints = _selectContactPointsComponent.ActiveContactPoints;
				_replaceContactPointsComponent.DeactivatedContactPoints = _selectContactPointsComponent.DeactivatedContactPoints;

				if (_replaceContactPointsComponent.DeactivatedContactPoints.Count == 0)
					Forward();
			}
			else if (currentComponent == _confirmationComponent)
			{
				_mergedPractitioner.PractitionerRef = _originalPractitionerRef;
				_mergePropertiesComponent.Save(_mergedPractitioner);
				_selectContactPointsComponent.Save(_mergedPractitioner);
				_replaceContactPointsComponent.Save();
				_confirmationComponent.PractitionerDetail = _mergedPractitioner;

				// The accept is enabled only on the very last page.
				this.AcceptEnabled = true;
			}
		}

		private void OnMovedBackward()
		{
			// The accept is enabled only on the very last page.  Nothing else to do when moving backward.
			this.AcceptEnabled = false;

			if (this.CurrentPage.Component == _replaceContactPointsComponent)
			{
				if (_replaceContactPointsComponent.DeactivatedContactPoints.Count == 0)
					Back();
			}
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
