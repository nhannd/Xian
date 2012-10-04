#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// ProcedureViewItem entity
	/// </summary>
	public partial class ProcedureSearchViewItem
	{

		public virtual void InitializeFromWorklistViewItem(WorklistViewItemBase other)
		{
			this.PatientProfile = other.PatientProfile;
			this.Procedure = other.Procedure;
			this.Order = other.Order;
			this.Visit = other.Visit;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}