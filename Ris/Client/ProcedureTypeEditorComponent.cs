#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ProcedureTypeEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ProcedureTypeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProcedureTypeEditorComponent class.
	/// </summary>
	[AssociateView(typeof(ProcedureTypeEditorComponentViewExtensionPoint))]
	public class ProcedureTypeEditorComponent : ApplicationComponent
	{
		private readonly EntityRef _procedureTypeRef;
		private ProcedureTypeDetail _procedureTypeDetail;
		private readonly bool _isNew;

		private ProcedureTypeSummary _procedureTypeSummary;
		private List<ModalitySummary> _modalityChoices;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProcedureTypeEditorComponent()
		{
			_isNew = true;
		}

		public ProcedureTypeEditorComponent(EntityRef procedureTypeRef)
		{
			_isNew = false;
			_procedureTypeRef = procedureTypeRef;
		}

		/// <summary>
		/// After editing is complete, gets the summary of the created/edited procedure type.
		/// </summary>
		public ProcedureTypeSummary ProcedureTypeSummary
		{
			get { return _procedureTypeSummary; }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			Platform.GetService<IProcedureTypeAdminService>(
				service =>
					{
						var formDataResponse = service.LoadProcedureTypeEditorFormData(new LoadProcedureTypeEditorFormDataRequest());
						_modalityChoices = formDataResponse.ModalityChoices;

						if (_isNew)
						{
							_procedureTypeDetail = new ProcedureTypeDetail();
						}
						else
						{
							var response = service.LoadProcedureTypeForEdit(new LoadProcedureTypeForEditRequest(_procedureTypeRef));
							_procedureTypeDetail = response.ProcedureType;
						}
					});

			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		public string ID
		{
			get { return _procedureTypeDetail.Id; }
			set
			{
				_procedureTypeDetail.Id = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _procedureTypeDetail.Name; }
			set
			{
				_procedureTypeDetail.Name = value;
				this.Modified = true;
			}
		}

		[ValidateGreaterThan(1)]
		public int DefaultDuration
		{
			get { return _procedureTypeDetail.DefaultDuration; }
			set 
			{
				_procedureTypeDetail.DefaultDuration = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public ModalitySummary DefaultModality
		{
			get { return _procedureTypeDetail.DefaultModality; }
			set
			{
				_procedureTypeDetail.DefaultModality = value;
				this.Modified = true;
			}
		}

		public IList DefaultModalityChoices
		{
			get { return _modalityChoices; }
		}

		public string FormatModalityItem(object item)
		{
			var summary = (ModalitySummary)item;
			return string.Format("{0} - {1}", summary.Id, summary.Name);
		}

		public ProcedureTypeSummary BaseType
		{
			get { return _procedureTypeDetail.BaseType; }
			set
			{
				_procedureTypeDetail.BaseType = value;
				this.Modified = true;
			}
		}

		public string FormatBaseTypeItem(object item)
		{
			var summary = (ProcedureTypeSummary)item;
			return string.Format("{0} - {1}", summary.Id, summary.Name);
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		public void Accept()
		{
			// editor does not support custom plans right now
			_procedureTypeDetail.CustomProcedurePlan = false;

			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				SaveChanges();
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionSaveProcedureType, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}


		#endregion

		private void SaveChanges()
		{
			Platform.GetService<IProcedureTypeAdminService>(
				service =>
					{
						if (_isNew)
						{
							var response = service.AddProcedureType(new AddProcedureTypeRequest(_procedureTypeDetail));
							_procedureTypeSummary = response.ProcedureType;
						}
						else
						{
							var response =
								service.UpdateProcedureType(new UpdateProcedureTypeRequest(_procedureTypeDetail));
							_procedureTypeSummary = response.ProcedureType;
						}
					});
		}


	}
}
