#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffOrStaffGroupSummaryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffOrStaffGroupSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// StaffOrStaffGroupSummaryComponent class.
	/// </summary>
	[AssociateView(typeof(StaffOrStaffGroupSummaryComponentViewExtensionPoint))]
	public class StaffOrStaffGroupSummaryComponent : ApplicationComponent
	{
		private ChildComponentHost _tabComponentContainerHost;
		private TabComponentContainer _tabComponentContainer;

		private StaffSummaryComponent _staffSummaryComponent;
		private StaffGroupSummaryComponent _staffGroupSummaryComponent;

		public override void Start()
		{
			_staffSummaryComponent = new StaffSummaryComponent {IncludeDeactivatedItems = false, HostedMode = true};
			_staffSummaryComponent.SummarySelectionChanged += OnSummaryComponentSummarySelectionChanged;
			_staffSummaryComponent.ItemDoubleClicked += OnSummaryComponentItemDoubleClicked;

			_staffGroupSummaryComponent = new StaffGroupSummaryComponent {IncludeDeactivatedItems = false, HostedMode = true};
			_staffGroupSummaryComponent.SummarySelectionChanged += OnSummaryComponentSummarySelectionChanged;
			_staffGroupSummaryComponent.ItemDoubleClicked += OnSummaryComponentItemDoubleClicked;

			_tabComponentContainer = new TabComponentContainer();
			_tabComponentContainer.Pages.Add(new TabPage(SR.TitleStaff, _staffSummaryComponent));
			_tabComponentContainer.Pages.Add(new TabPage(SR.TitleStaffGroups, _staffGroupSummaryComponent));

			_tabComponentContainerHost = new ChildComponentHost(this.Host, _tabComponentContainer);
			_tabComponentContainerHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_tabComponentContainerHost != null)
			{
				_tabComponentContainerHost.StopComponent();
				_tabComponentContainerHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ApplicationComponentHost TabComponentContainerHost
		{
			get { return _tabComponentContainerHost; }
		}

		public StaffSummary SelectedStaff
		{
			get { return (StaffSummary) _staffSummaryComponent.SummarySelection.Item; }
		}

		public StaffGroupSummary SelectedStaffGroup
		{
			get { return (StaffGroupSummary) _staffGroupSummaryComponent.SummarySelection.Item; }
		}

		public bool IsSelectingStaff
		{
			get { return _tabComponentContainer.CurrentPage.Component == _staffSummaryComponent; }
		}

		public bool IsSelectingStaffGroup
		{
			get { return _tabComponentContainer.CurrentPage.Component == _staffGroupSummaryComponent; }
		}

		public bool AcceptEnabled
		{
			get 
			{
				return this.IsSelectingStaff && _staffSummaryComponent.AcceptEnabled
					|| this.IsSelectingStaffGroup && _staffGroupSummaryComponent.AcceptEnabled;
			}
		}

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		#region Private Methods

		private void OnSummaryComponentSummarySelectionChanged(object sender, System.EventArgs e)
		{
			NotifyPropertyChanged("AcceptEnabled");
		}

		private void OnSummaryComponentItemDoubleClicked(object sender, System.EventArgs e)
		{
			Accept();
		}

		#endregion
	}
}
