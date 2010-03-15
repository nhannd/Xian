using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerMergeNavigatorComponent : NavigatorComponentContainer
	{
		private ExternalPractitionerMergeSelectedDuplicateComponent _mergeSelectedDuplicateComponent;
		private ExternalPractitionerMergePropertiesComponent _mergePropertiesComponent;
		private ExternalPractitionerMergeSelectedContactPointsComponent _selectContactPointsComponent;
		private ExternalPractitionerMergeSelectDefaultContactPointComponent _selectDefaultContactPointComponent;
		private ExternalPractitionerMergeAffectedOrdersComponent _affectedOrdersComponent;

		private EntityRef _originalPractitionerRef;
		private ExternalPractitionerDetail _originalPractitioner;
		private readonly ExternalPractitionerDetail _mergedPractitioner;

		private List<ExternalPractitionerSummary> _duplicates;

		public ExternalPractitionerMergeNavigatorComponent(EntityRef practitionerRef)
		{
			_originalPractitionerRef = practitionerRef;
			_mergedPractitioner = new ExternalPractitionerDetail();
		}

		public override void Start()
		{
			this.Pages.Add(new NavigatorPage("Select Duplicate", _mergeSelectedDuplicateComponent = new ExternalPractitionerMergeSelectedDuplicateComponent()));
			this.Pages.Add(new NavigatorPage("Resolve Property Conflicts", _mergePropertiesComponent = new ExternalPractitionerMergePropertiesComponent()));
			this.Pages.Add(new NavigatorPage("Choose Contact Points", _selectContactPointsComponent = new ExternalPractitionerMergeSelectedContactPointsComponent()));
			this.Pages.Add(new NavigatorPage("Choose Default Contact Point", _selectDefaultContactPointComponent = new ExternalPractitionerMergeSelectDefaultContactPointComponent()));
			this.Pages.Add(new NavigatorPage("Resolve Order Conflicts", _affectedOrdersComponent = new ExternalPractitionerMergeAffectedOrdersComponent()));
			this.ValidationStrategy = new AllComponentsValidationStrategy();

			base.Start();
		}

		public override bool ShowTree
		{
			// Disable tree pane, so user can only navigate with the Forward and Backward buttons.
			get { return false; }
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
			if (currentComponent == _mergeSelectedDuplicateComponent)
			{
				// On initialization, page move forward to the first page.  Load detail and duplicates.
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest(_originalPractitionerRef) { IncludeDetail = true, IncludeDuplicates = true };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						_originalPractitionerRef = response.PractitionerDetail.PractitionerRef;
						_originalPractitioner = response.PractitionerDetail;
						_duplicates = response.Duplicates;
					});

				_mergeSelectedDuplicateComponent.ExternalPractitioners = _duplicates;

				// Disable forward/backward enablement, unless an external practitioner is selected.
				this.BackEnabled = false;
				this.ForwardEnabled = false;
				_mergeSelectedDuplicateComponent.SummarySelectionChanged += delegate 
					{ this.ForwardEnabled = _mergeSelectedDuplicateComponent.SelectedPractitioner != null; };
			}
			else if (currentComponent == _mergePropertiesComponent)
			{
				// Load detail of the selected practitioner.
				ExternalPractitionerDetail selectedDuplicatePractitioner = null;
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest(_mergeSelectedDuplicateComponent.SelectedPractitioner.PractitionerRef) { IncludeDetail = true };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						selectedDuplicatePractitioner = response.PractitionerDetail;
					});

				_mergedPractitioner.PractitionerRef = _originalPractitionerRef;
				_mergePropertiesComponent.OriginalPractitioner = _originalPractitioner;
				_mergePropertiesComponent.DuplicatePractitioner = selectedDuplicatePractitioner;

				// Update forward/backward enablement
				this.BackEnabled = true;
				this.ForwardEnabled = true;
			}
			else if (currentComponent == _selectContactPointsComponent)
			{
				_mergePropertiesComponent.Save(_mergedPractitioner);
			}
			else if (currentComponent == _selectDefaultContactPointComponent)
			{
				
			}
			else if (currentComponent == _affectedOrdersComponent)
			{
				
			}
		}

		private void OnMovedBackward()
		{
			var currentComponent = this.CurrentPage.Component;
			if (currentComponent == _mergeSelectedDuplicateComponent)
			{
			}
			else if (currentComponent == _mergePropertiesComponent)
			{

			}
			else if (currentComponent == _selectContactPointsComponent)
			{

			}
			else if (currentComponent == _selectDefaultContactPointComponent)
			{

			}
			else if (currentComponent == _affectedOrdersComponent)
			{

			}
		}

	}
}
