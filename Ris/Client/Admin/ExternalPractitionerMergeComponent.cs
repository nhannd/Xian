using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/Admin/ExternalPractitioner/Merge Duplicates", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ExternalPractitioner)]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ExternalPractitionerMergeTool : Tool<IDesktopToolContext>
	{
		public void Launch()
		{
			try
			{
				ExternalPractitionerMergeComponent component = new ExternalPractitionerMergeComponent();

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitleMergeExternalPractitioner);
			}
			catch (Exception e)
			{
				// failed to launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ExternalPractitionerMergeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeComponent class
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeComponent : SummaryComponentBase<ExternalPractitionerSummary, ExternalPractitionerTable>
	{
		private ExternalPractitionerSummary _selectedPractitionerSummary;

		#region Presentation Model

		public string FamilyName
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.FamilyName; }
		}

		public string GivenName
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.GivenName; }
		}

		public string MiddleName
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.MiddleName; }
		}

		public string Prefix
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.Prefix; }
		}

		public string Suffix
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.Suffix; }
		}

		public string Degree
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.Name.Degree; }
		}

		public string LicenseNumber
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.LicenseNumber; }
		}

		public string BillingNumber
		{
			get { return _selectedPractitionerSummary == null ? null : _selectedPractitionerSummary.BillingNumber; }
		}

		#endregion

		#region Overrides

		protected override bool SupportsEdit
		{
			get { return false; }
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		public override bool AcceptEnabled
		{
			get { return this.SummarySelection.Items.Length == 1; }
		}

		protected override IList<ExternalPractitionerSummary> ListItems(int firstItem, int maxItems)
		{
			// starts with an empty list
			return new List<ExternalPractitionerSummary>();
		}

		protected override bool AddItems(out IList<ExternalPractitionerSummary> addedItems)
		{
			addedItems = new List<ExternalPractitionerSummary>();
			ExternalPractitionerSummaryComponent summaryComponent = new ExternalPractitionerSummaryComponent(true);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, summaryComponent, SR.TitleMergeExternalPractitioner);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems = CollectionUtils.Map<object, ExternalPractitionerSummary>(
					summaryComponent.SummarySelection.Items,
					delegate(object item)
					{
						return (ExternalPractitionerSummary)item;
					});

				return true;
			}
			return false;
		}

		protected override bool EditItems(IList<ExternalPractitionerSummary> items, out IList<ExternalPractitionerSummary> editedItems)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override bool DeleteItems(IList<ExternalPractitionerSummary> items)
		{
			return true;
		}

		protected override bool IsSameItem(ExternalPractitionerSummary x, ExternalPractitionerSummary y)
		{
			return x.PractitionerRef.Equals(y.PractitionerRef, true);
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			if (this.SelectedItems.Count == 1)
				_selectedPractitionerSummary = (ExternalPractitionerSummary) this.SummarySelection.Item;
			else
				_selectedPractitionerSummary = null;

			NotifyAllPropertiesChanged();
		}

		public override void Accept()
		{
			try
			{
				ExternalPractitionerSummary original = (ExternalPractitionerSummary)this.SummarySelection.Item;
				List<ExternalPractitionerSummary> duplicates = CollectionUtils.Map<object, ExternalPractitionerSummary>(
					this.SummarySelection.Items,
					delegate(object item)
					{
						return (ExternalPractitionerSummary)item;
					});

				Platform.GetService<IExternalPractitionerAdminService>(
					delegate(IExternalPractitionerAdminService service)
					{
						MergeDuplicatePractitionerRequest request = new MergeDuplicatePractitionerRequest(original, duplicates);
						service.MergeDuplicatePractitioner(request);
					});

				base.Accept();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToMergeDuplicatePractitioners, this.Host.DesktopWindow);
			}
		}

		#endregion
	}
}
