#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{

	/// <summary>
	/// Extension point for views onto <see cref="DiagnosticServiceEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DiagnosticServiceEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DiagnosticServiceEditorComponent class
	/// </summary>
	[AssociateView(typeof(DiagnosticServiceEditorComponentViewExtensionPoint))]
	public class DiagnosticServiceEditorComponent : ApplicationComponent
	{
		private readonly bool _isNew;

		private EntityRef _editedItemEntityRef;
		private DiagnosticServiceDetail _editedItemDetail;
		private DiagnosticServiceSummary _editedItemSummary;

		private ProcedureTypeSummaryTable _availableDiagnosticServices;
		private ProcedureTypeSummaryTable _selectedDiagnosticServices;

		public DiagnosticServiceEditorComponent()
		{
			_isNew = true;
		}

		public DiagnosticServiceEditorComponent(EntityRef entityRef)
		{
			_editedItemEntityRef = entityRef;
			_isNew = false;
		}

		public override void Start()
		{
			_availableDiagnosticServices = new ProcedureTypeSummaryTable();
			_selectedDiagnosticServices = new ProcedureTypeSummaryTable();

			Platform.GetService<IDiagnosticServiceAdminService>(
				delegate(IDiagnosticServiceAdminService service)
				{
					LoadDiagnosticServiceEditorFormDataResponse formDataResponse =
						service.LoadDiagnosticServiceEditorFormData(new LoadDiagnosticServiceEditorFormDataRequest());
					_availableDiagnosticServices.Items.AddRange(formDataResponse.ProcedureTypeChoices);

					if (_isNew)
					{
						_editedItemDetail = new DiagnosticServiceDetail();
					}
					else
					{
						LoadDiagnosticServiceForEditResponse response =
							service.LoadDiagnosticServiceForEdit(
								new LoadDiagnosticServiceForEditRequest(_editedItemEntityRef));

						_editedItemDetail = response.DiagnosticService;
						_selectedDiagnosticServices.Items.AddRange(_editedItemDetail.ProcedureTypes);
					}

					foreach (ProcedureTypeSummary selectedSummary in _selectedDiagnosticServices.Items)
					{
						_availableDiagnosticServices.Items.Remove(selectedSummary);
					}
				});

			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public DiagnosticServiceSummary DiagnosticServiceSummary
		{
			get { return _editedItemSummary; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _editedItemDetail.Name; }
			set
			{
				_editedItemDetail.Name = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string ID
		{
			get { return _editedItemDetail.Id; }
			set
			{
				_editedItemDetail.Id = value;
				this.Modified = true;
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public ITable AvailableDiagnosticServices
		{
			get { return _availableDiagnosticServices; }
		}

		public ITable SelectedDiagnosticServices
		{
			get { return _selectedDiagnosticServices; }
		}

		#endregion

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					Platform.GetService<IDiagnosticServiceAdminService>(
						delegate(IDiagnosticServiceAdminService service)
						{
							if (_isNew)
							{
								_editedItemDetail.ProcedureTypes.AddRange(_selectedDiagnosticServices.Items);
								AddDiagnosticServiceResponse response =
									service.AddDiagnosticService(new AddDiagnosticServiceRequest(_editedItemDetail));
								_editedItemEntityRef = response.DiagnosticService.DiagnosticServiceRef;
								_editedItemSummary = response.DiagnosticService;
							}
							else
							{
								_editedItemDetail.ProcedureTypes.Clear();
								_editedItemDetail.ProcedureTypes.AddRange(_selectedDiagnosticServices.Items);
								UpdateDiagnosticServiceResponse response =
									service.UpdateDiagnosticService(new UpdateDiagnosticServiceRequest(_editedItemDetail));
								_editedItemEntityRef = response.DiagnosticService.DiagnosticServiceRef;
								_editedItemSummary = response.DiagnosticService;
							}
						});

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveDiagnosticService, this.Host.DesktopWindow,
						delegate()
						{
							this.ExitCode = ApplicationComponentExitCode.Error;
							this.Host.Exit();
						});
				}
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		public void ItemsAddedOrRemoved()
		{
			this.Modified = true;
		}
	}
}
