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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ProtocolEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProtocolEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocolEditorComponent class
	/// </summary>
	[AssociateView(typeof(ProtocolEditorComponentViewExtensionPoint))]
	public class ProtocolEditorComponent : ApplicationComponent
	{
		internal interface IDefaultProtocolGroupSettingsProvider
		{
			string this[string procedureName] { get; set; }
			string GetSuggestedDefault();
		}

		private class DefaultProtocolGroupSettingsProvider : IDefaultProtocolGroupSettingsProvider
		{
			#region IDefaultProtocolGroupSettingsProvider Members

			public string this[string procedureName]
			{
				get
				{
					return ProtocolGroupSettings.Default.GetDefaultProtocolGroup(procedureName);
				}
				set
				{
					if (String.IsNullOrEmpty(value)) return;

					ProtocolGroupSettings.Default.SetDefaultProtocolGroup(value, procedureName);
				}
			}

			public string GetSuggestedDefault()
			{
				return ProtocolGroupSettings.Default.LastDefaultProtocolGroup;
			}

			#endregion
		}

		#region Private Fields

		private ReportingWorklistItem _worklistItem;

		private List<EnumValueInfo> _protocolUrgencyChoices;

		private readonly ProtocolEditorProcedurePlanSummaryTable _procedurePlanSummaryTable;
		private ProtocolEditorProcedurePlanSummaryTableItem _selectedProcodurePlanSummaryTableItem;
		private event EventHandler _selectedProcedurePlanSummaryTableItemChanged;

		private List<ProtocolGroupSummary> _protocolGroupChoices;
		private ProtocolGroupSummary _protocolGroup;
		private string _defaultProtocolGroupName;
		private readonly IDefaultProtocolGroupSettingsProvider _defaultProtocolGroupProvider = new DefaultProtocolGroupSettingsProvider();

		private readonly ProtocolCodeTable _availableProtocolCodes;
		private readonly ProtocolCodeTable _selectedProtocolCodes;
		private ProtocolCodeDetail _selectedProtocolCodesSelection;
		private bool _canEdit;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public ProtocolEditorComponent(ReportingWorklistItem worklistItem)
		{
			_worklistItem = worklistItem;

			_availableProtocolCodes = new ProtocolCodeTable();
			_selectedProtocolCodes = new ProtocolCodeTable();
			_selectedProtocolCodes.Items.ItemsChanged += SelectedProtocolCodesChanged;
			_protocolGroupChoices = new List<ProtocolGroupSummary>();
			_procedurePlanSummaryTable = new ProtocolEditorProcedurePlanSummaryTable();
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
					{
						GetProtocolFormDataResponse response = service.GetProtocolFormData(new GetProtocolFormDataRequest());
						_protocolUrgencyChoices = response.ProtocolUrgencyChoices;

						LoadWorklistItem(service);
					});

			base.Start();
		}

		#endregion

		public ReportingWorklistItem WorklistItem
		{
			get { return _worklistItem; }
			set
			{
				_worklistItem = value;

				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						LoadWorklistItem(service);
					});
			}
		}

		private void LoadWorklistItem(IProtocollingWorkflowService service)
		{
			if (_worklistItem == null)
				return;

			GetProcedurePlanForProtocollingWorklistItemRequest procedurePlanRequest = 
				new GetProcedurePlanForProtocollingWorklistItemRequest(_worklistItem.ProcedureStepRef);

			GetProcedurePlanForProtocollingWorklistItemResponse procedurePlanResponse = 
				service.GetProcedurePlanForProtocollingWorklistItem(procedurePlanRequest);

			if (procedurePlanResponse.ProcedurePlan != null)
			{
				_procedurePlanSummaryTable.Items.Clear();

				foreach (ProcedureDetail rp in procedurePlanResponse.ProcedurePlan.Procedures)
				{
					GetProcedureProtocolRequest protocolRequest = new GetProcedureProtocolRequest(rp.ProcedureRef);
					GetProcedureProtocolResponse protocolResponse = service.GetProcedureProtocol(protocolRequest);

					if (protocolResponse.ProtocolDetail != null)
					{
						_procedurePlanSummaryTable.Items.Add(
							new ProtocolEditorProcedurePlanSummaryTableItem(rp, protocolResponse.ProtocolDetail));
					}
				}
				_procedurePlanSummaryTable.Sort();
			}
		}

		#region Presentation Model

		public string Author
		{
			get
			{
				return _selectedProcodurePlanSummaryTableItem != null 
					? PersonNameFormat.Format(_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Author.Name)
					: string.Empty;
			}
		}

		public bool ShowAuthor
		{
			// TODO: do not show name if it's the current user
			get { return true; }
		}

		public string Urgency
		{
			get
			{
				if (_selectedProcodurePlanSummaryTableItem != null &&
					_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Urgency != null)
				{
					return _selectedProcodurePlanSummaryTableItem.ProtocolDetail.Urgency.Value;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Urgency = EnumValueUtils.MapDisplayValue(_protocolUrgencyChoices, value);
				this.Modified = true;
			}
		}

		public List<string> UrgencyChoices
		{
			get { return EnumValueUtils.GetDisplayValues(_protocolUrgencyChoices); }
		}

		public IList<string> ProtocolGroupChoices
		{
			get
			{
				return CollectionUtils.Map<ProtocolGroupSummary, string>(
					_protocolGroupChoices,
					delegate(ProtocolGroupSummary summary) { return AppendDefaultText(summary.Name); });
			}
		}

		public string ProtocolGroup
		{
			get { return _protocolGroup == null ? "" : AppendDefaultText(_protocolGroup.Name); }
			set
			{
				_protocolGroup = (value == null)
									? null
									: CollectionUtils.SelectFirst<ProtocolGroupSummary>(
										_protocolGroupChoices,
										delegate(ProtocolGroupSummary summary) { return summary.Name == RemoveDefaultText(value); });

				ProtocolGroupSelectionChanged();
			}
		}

		public bool SetDefaultProtocolGroupEnabled
		{
			get
			{
				if (_protocolGroup == null) return false;

				return _defaultProtocolGroupName != _protocolGroup.Name;
			}
		}

		public void SetDefaultProtocolGroup()
		{
			_defaultProtocolGroupProvider[_selectedProcodurePlanSummaryTableItem.ProcedureDetail.Type.Name] = _defaultProtocolGroupName = _protocolGroup.Name;

			NotifyPropertyChanged("ProtocolGroupChoices");
			NotifyPropertyChanged("ProtocolGroup");
			NotifyPropertyChanged("SetDefaultProtocolGroupEnabled");
		}

		public ITable AvailableProtocolCodesTable
		{
			get { return _availableProtocolCodes; }
		}

		public ITable SelectedProtocolCodesTable
		{
			get { return _selectedProtocolCodes; }
		}

		// this isn't actually used by the component, but we need it for validation
		// so that we can show an error icon next to the "selected items" table
		public ISelection SelectedProtocolCodesSelection
		{
			get { return new Selection(_selectedProtocolCodesSelection); }
			set
			{
				_selectedProtocolCodesSelection = (ProtocolCodeDetail)value.Item;
			}
		}

		public ITable ProcedurePlanSummaryTable
		{
			get { return _procedurePlanSummaryTable; }
		}

		public ISelection SelectedProcedure
		{
			get { return new Selection(_selectedProcodurePlanSummaryTableItem); }
			set
			{
				ProtocolEditorProcedurePlanSummaryTableItem item = (ProtocolEditorProcedurePlanSummaryTableItem)value.Item;
				ProcedureSelectionChanged(item);
			}
		}

		public event EventHandler SelectedProcedureChanged
		{
			add { _selectedProcedurePlanSummaryTableItemChanged += value; }
			remove { _selectedProcedurePlanSummaryTableItemChanged -= value; }
		}

		public bool CanEdit
		{
			get { return _canEdit; }
			internal set { _canEdit = value; }
		}


		#endregion

		#region Private Methods

		private void ProcedureSelectionChanged(ProtocolEditorProcedurePlanSummaryTableItem item)
		{
			// Same selection, do nothing
			if (item == _selectedProcodurePlanSummaryTableItem)
			{
				return;
			}

			ResetDocument();

			// Ensure something is selected
			if (item != null)
			{
				//Refresh protocol
				try
				{
					Platform.GetService<IProtocollingWorkflowService>(
						delegate(IProtocollingWorkflowService service)
							{
								// Load available protocol groups
								ListProtocolGroupsForProcedureRequest request = new ListProtocolGroupsForProcedureRequest(item.ProcedureDetail.ProcedureRef);
								ListProtocolGroupsForProcedureResponse response = service.ListProtocolGroupsForProcedure(request);

								_protocolGroupChoices = response.ProtocolGroups;
								//_protocolGroup = response.InitialProtocolGroup;
								_protocolGroup = GetInitialProtocolGroup(item);

								RefreshAvailableProtocolCodes(item.ProtocolDetail.Codes, service);

								// fill out selected item codes
								_selectedProtocolCodes.Items.Clear();
								_selectedProtocolCodes.Items.AddRange(item.ProtocolDetail.Codes);
							});
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Host.DesktopWindow);
				}
			}

			_selectedProcodurePlanSummaryTableItem = item;
			EventsHelper.Fire(_selectedProcedurePlanSummaryTableItemChanged, this, EventArgs.Empty);

			NotifyPropertyChanged("ProtocolGroupChoices");
			//NotifyPropertyChanged("Urgency");
		}

		private void RefreshAvailableProtocolCodes(IEnumerable<ProtocolCodeDetail> existingSelectedCodes, IProtocollingWorkflowService service)
		{
			_availableProtocolCodes.Items.Clear();

			if (_protocolGroup != null)
			{
				GetProtocolGroupDetailRequest protocolCodesDetailRequest = new GetProtocolGroupDetailRequest(_protocolGroup);
				GetProtocolGroupDetailResponse protocolCodesDetailResponse = service.GetProtocolGroupDetail(protocolCodesDetailRequest);

				_availableProtocolCodes.Items.AddRange(protocolCodesDetailResponse.ProtocolGroup.Codes);

				// Make existing code selections unavailable
				foreach (ProtocolCodeDetail code in existingSelectedCodes)
				{
					_availableProtocolCodes.Items.Remove(code);
				}
			}
		}

		private void SelectedProtocolCodesChanged(object sender, ItemChangedEventArgs e)
		{
			ProtocolCodeDetail detail = (ProtocolCodeDetail)e.Item;
			switch (e.ChangeType)
			{
				case ItemChangeType.ItemAdded:
					_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes.Add(detail);
					break;
				case ItemChangeType.ItemRemoved:
					_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes.Remove(detail);
					break;
				default:
					return;
			}

			this.Modified = true;
		}

		private ProtocolGroupSummary GetInitialProtocolGroup(ProtocolEditorProcedurePlanSummaryTableItem item)
		{
			ProtocolGroupSummary defaultProtocolGroup = null;

			// Use the default if one exists for this procedure.
			// Otherwise, get a suggested initial group and set the default to the suggested value
			_defaultProtocolGroupName = _defaultProtocolGroupProvider[item.ProcedureDetail.Type.Name]
									?? (_defaultProtocolGroupProvider[item.ProcedureDetail.Type.Name] = GetSuggestedDefault());

			if (string.IsNullOrEmpty(_defaultProtocolGroupName) == false)
			{
				defaultProtocolGroup = CollectionUtils.SelectFirst<ProtocolGroupSummary>(
					_protocolGroupChoices,
					delegate(ProtocolGroupSummary summary) { return summary.Name == _defaultProtocolGroupName; });
			}

			defaultProtocolGroup = defaultProtocolGroup ?? CollectionUtils.FirstElement(_protocolGroupChoices);

			return defaultProtocolGroup;
		}

		private string GetSuggestedDefault()
		{
			if (CollectionUtils.Contains(
				_protocolGroupChoices,
				delegate(ProtocolGroupSummary pgs) { return pgs.Name == _defaultProtocolGroupProvider.GetSuggestedDefault(); }))
			{
				return _defaultProtocolGroupProvider.GetSuggestedDefault();
			}
			else
			{
				return null;
			}
		}

		private void ProtocolGroupSelectionChanged()
		{
			try
			{
				Platform.GetService<IProtocollingWorkflowService>(
					delegate(IProtocollingWorkflowService service)
					{
						RefreshAvailableProtocolCodes(_selectedProcodurePlanSummaryTableItem.ProtocolDetail.Codes, service);
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		// Refresh the list of available protocol codes when the list of protocol groups is initially loaded 
		// and whenever the protocol group selection changes
		private string AppendDefaultText(string value)
		{
			return value == _defaultProtocolGroupName ? value + " (Default)" : value;
		}

		private static string RemoveDefaultText(string value)
		{
			if (value.EndsWith(" (Default)"))
				value.Replace(" (Default)", "");

			return value;
		}

		private void ResetDocument()
		{
			_protocolGroup = null;
			_protocolGroupChoices = new List<ProtocolGroupSummary>();
			_availableProtocolCodes.Items.Clear();
			_selectedProtocolCodes.Items.Clear();
		}

		#endregion
	}
}
