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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	/// <summary>
	/// Extension point for views onto <see cref="DepartmentEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DepartmentEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DepartmentEditorComponent class.
	/// </summary>
	[AssociateView(typeof(DepartmentEditorComponentViewExtensionPoint))]
	public class DepartmentEditorComponent : ApplicationComponent
	{
		private List<FacilitySummary> _facilityChoices;

		private DepartmentDetail _detail;
		private EntityRef _itemRef;
		private readonly bool _isNew;

		private DepartmentSummary _summary;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DepartmentEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DepartmentEditorComponent(EntityRef itemRef)
		{
			_isNew = false;
			_itemRef = itemRef;
		}

		public DepartmentSummary DepartmentSummary
		{
			get { return _summary; }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			Platform.GetService<IDepartmentAdminService>(
				service =>
				{
					var formDataResponse = service.LoadDepartmentEditorFormData(
						new LoadDepartmentEditorFormDataRequest());
					_facilityChoices = formDataResponse.FacilityChoices;

					if (_isNew)
					{
						_detail = new DepartmentDetail();
					}
					else
					{
						var response = service.LoadDepartmentForEdit(new LoadDepartmentForEditRequest(_itemRef));
						_itemRef = response.Department.DepartmentRef;
						_detail = response.Department;
					}
				});

			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Id
		{
			get { return _detail.Id; }
			set
			{
				_detail.Id = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _detail.Name; }
			set
			{
				_detail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _detail.Description; }
			set
			{
				_detail.Description = value;
				this.Modified = true;
			}
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		[ValidateNotNull]
		public FacilitySummary Facility
		{
			get { return _detail.Facility; }
			set
			{
				_detail.Facility = value;
				this.Modified = true;
			}
		}

		public string FormatFacility(object item)
		{
			var f = (FacilitySummary)item;
			return f.Name;
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
					Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionSaveLocation, this.Host.DesktopWindow,
					                        () => Exit(ApplicationComponentExitCode.Error));
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

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		#endregion

		private void SaveChanges()
		{
			if (_isNew)
			{
				Platform.GetService<IDepartmentAdminService>(
					service =>
					{
						var response = service.AddDepartment(new AddDepartmentRequest(_detail));
						_itemRef = response.Department.DepartmentRef;
						_summary = response.Department;
					});
			}
			else
			{
				Platform.GetService<IDepartmentAdminService>(
					service =>
					{
						var response = service.UpdateDepartment(new UpdateDepartmentRequest(_detail));
						_itemRef = response.Department.DepartmentRef;
						_summary = response.Department;
					});
			}
		}
	}
}
