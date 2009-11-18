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
			_staffSummaryComponent = new StaffSummaryComponent();
			_staffSummaryComponent.HostedMode = true;
			_staffSummaryComponent.SummarySelectionChanged += OnSummaryComponentSummarySelectionChanged;
			_staffSummaryComponent.ItemDoubleClicked += OnSummaryComponentItemDoubleClicked;

			_staffGroupSummaryComponent = new StaffGroupSummaryComponent();
			_staffGroupSummaryComponent.HostedMode = true;
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
