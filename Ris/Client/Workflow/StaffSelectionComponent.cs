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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffSelectionComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// SupervisorSelectionComponent class.
	/// </summary>
	[AssociateView(typeof(StaffSelectionComponentViewExtensionPoint))]
	public abstract class StaffSelectionComponent : ApplicationComponent
	{
		#region Private Methods

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _staff;

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, this.StaffTypes);
			_staff = !string.IsNullOrEmpty(this.DefaultSupervisorID) ? GetStaffByID(this.DefaultSupervisorID) : null;

			base.Start();
		}

		#endregion

		#region Presentation model

		public StaffSummary Staff
		{
			get { return _staff; }
			set
			{
				if (!Equals(value, _staff))
				{
					SetStaff(value);
					NotifyPropertyChanged("Staff");
				}
			}
		}

		public ILookupHandler StaffLookupHandler
		{
			get { return _staffLookupHandler; }
		}

		public abstract string LabelText { get; }

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		protected abstract string[] StaffTypes { get; }
		protected abstract string DefaultSupervisorID { get; }

		protected virtual void SetStaff(StaffSummary staff)
		{
			_staff = staff;
		}

		private StaffSummary GetStaffByID(string id)
		{
			StaffSummary staff = null;
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					ListStaffResponse response = service.ListStaff(
						new ListStaffRequest(id, null, null, null));
					staff = CollectionUtils.FirstElement(response.Staffs);
				});
			return staff;
		}
	}
}
