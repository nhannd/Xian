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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="PatientSearchComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class PatientSearchToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IPatientSearchToolContext : IToolContext
	{
		event EventHandler SelectedProfileChanged;
		PatientProfileSummary SelectedProfile { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// PatientSearchComponent class
	/// </summary>
	[AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
	public class PatientSearchComponent : ApplicationComponent
	{
		class PatientSearchToolContext : ToolContext, IPatientSearchToolContext
		{
			private readonly PatientSearchComponent _component;

			public PatientSearchToolContext(PatientSearchComponent component)
			{
				_component = component;
			}

			public event EventHandler SelectedProfileChanged
			{
				add { _component.SelectedProfileChanged += value; }
				remove { _component.SelectedProfileChanged -= value; }
			}

			public PatientProfileSummary SelectedProfile
			{
				get { return (PatientProfileSummary)_component.SelectedProfile.Item; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}

		private string _searchString;

		private PatientProfileTable _profileTable;
		private PatientProfileSummary _selectedProfile;
		private event EventHandler _selectedProfileChanged;

		private ToolSet _toolSet;

		public override void Start()
		{
			_profileTable = new PatientProfileTable();
			_toolSet = new ToolSet(new PatientSearchToolExtensionPoint(), new PatientSearchToolContext(this));

			base.Start();
		}

		public override void Stop()
		{
			_toolSet.Dispose();

			base.Stop();
		}

		#region Presentation Model

		public string SearchString
		{
			get { return _searchString; }
			set { _searchString = value; }
		}

		public bool SearchEnabled
		{
			get { return !String.IsNullOrEmpty(_searchString); }
		}

		public ITable Profiles
		{
			get { return _profileTable; }
		}

		public ActionModelNode ItemsContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-contextmenu", _toolSet.Actions); }
		}

		public ActionModelNode ItemsToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-toolbar", _toolSet.Actions); }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		public ISelection SelectedProfile
		{
			get { return new Selection(_selectedProfile); }
			set
			{
				if (Equals(_selectedProfile, value.Item))
					return;

				_selectedProfile = (PatientProfileSummary)value.Item;
				EventsHelper.Fire(_selectedProfileChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SelectedProfileChanged
		{
			add { _selectedProfileChanged += value; }
			remove { _selectedProfileChanged -= value; }
		}

		public void Search()
		{
			try
			{
				_profileTable.Items.Clear();

				TextQueryResponse<PatientProfileSummary> response = null;

				Platform.GetService(
					delegate(IRegistrationWorkflowService service)
					{
						var request = new TextQueryRequest();
						request.TextQuery = _searchString;
						request.SpecificityThreshold = PatientSearchComponentSettings.Default.SearchCriteriaSpecificityThreshold;
						response = service.PatientProfileTextQuery(request);
					});

				if (response.TooManyMatches)
					throw new WeakSearchCriteriaException();

				_profileTable.Items.AddRange(response.Matches);
				this.SelectedProfile = new Selection(_profileTable.Items.Count > 0 ? _profileTable.Items[0] : null);

				if (_profileTable.Items.Count == 0)
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageResultsNotFound, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void OpenPatient()
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(_selectedProfile.PatientProfileRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(_selectedProfile.PatientRef, _selectedProfile.PatientProfileRef, this.Host.DesktopWindow);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion
	}
}
