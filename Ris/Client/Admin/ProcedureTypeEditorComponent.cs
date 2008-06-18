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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;
using System.Collections;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;

namespace ClearCanvas.Ris.Client.Admin
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
		private bool _isNew;

		private ProcedureTypeSummary _procedureTypeSummary;

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

		public ProcedureTypeDetail ProcedureTypeDetail
		{
			get { return _procedureTypeDetail; }
			set { _procedureTypeDetail = value; }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			if (_isNew)
			{
				_procedureTypeDetail = new ProcedureTypeDetail();
			}
			else
			{
				Platform.GetService<IProcedureTypeAdminService>(
					delegate(IProcedureTypeAdminService service)
					{
						LoadProcedureTypeForEditResponse response = service.LoadProcedureTypeForEdit(new LoadProcedureTypeForEditRequest(_procedureTypeRef));
						_procedureTypeDetail = response.ProcedureType;
					});
			}

			base.Start();
		}

		#region Presentation Model

		public string ID
		{
			get { return _procedureTypeDetail.Id; }
			set
			{
				_procedureTypeDetail.Id = value;
				this.Modified = true;
			}
		}

		public string Name
		{
			get { return _procedureTypeDetail.Name; }
			set
			{
				_procedureTypeDetail.Name = value;
				this.Modified = true;
			}
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

		public IList BaseTypeList
		{
			get
			{
				LoadProcedureTypeEditorFormDataResponse response = null;
				Platform.GetService<IProcedureTypeAdminService>(
					delegate(IProcedureTypeAdminService service)
					{
						response = service.LoadProcedureTypeEditorFormData(new LoadProcedureTypeEditorFormDataRequest());
					});
				return response.BaseProcedureTypeChoices;
			}
		}

		public string BaseTypeName
		{
			get { return _procedureTypeDetail.BaseType.Name; }
			set
			{
				_procedureTypeDetail.BaseType.Name = value;
				this.Modified = true;
			}
		}

		public string PlanXml
		{
			get { return _procedureTypeDetail.PlanXml; }
			set
			{
				_procedureTypeDetail.PlanXml = value;
				this.Modified = true;
			}
		}

		public string FormatBaseTypeItem(object item)
		{
			ProcedureTypeSummary summary = (ProcedureTypeSummary)item;
			return "("+summary.Id+")" + " " + summary.Name;
		}

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
					SaveChanges();
					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, "Unable to save procedure type", this.Host.DesktopWindow,
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

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		#endregion

		private void SaveChanges()
		{
			Platform.GetService<IProcedureTypeAdminService>(
				delegate(IProcedureTypeAdminService service)
				{
					if (_isNew)
					{
						AddProcedureTypeResponse response = service.AddProcedureType(new AddProcedureTypeRequest(_procedureTypeDetail));
						_procedureTypeSummary = response.ProcedureType;
					}
					else
					{
						UpdateProcedureTypeResponse response = service.UpdateProcedureType(new UpdateProcedureTypeRequest(_procedureTypeDetail));
						_procedureTypeSummary = response.ProcedureType;
					}
				});
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}
	}
}
