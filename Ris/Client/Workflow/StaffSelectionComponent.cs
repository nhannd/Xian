#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
						new ListStaffRequest(id, null, null, null, null, true));
					staff = CollectionUtils.FirstElement(response.Staffs);
				});
			return staff;
		}
	}
}
