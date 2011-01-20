#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents an institution's details.
	/// </summary>
	internal struct Institution
	{
		/// <summary>
		/// Gets or sets the institution's name.
		/// </summary>
		public string Name;

		/// <summary>
		/// Gets or sets the institution's address.
		/// </summary>
		public string Address;

		/// <summary>
		/// Gets or sets the institutional department's name.
		/// </summary>
		public string DepartmentName;

		/// <summary>
		/// Returns true if and only if all fields are exactly the same between this and <paramref name="obj">that</paramref>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Institution)
			{
				Institution other = (Institution) obj;
				return this.Name == other.Name && this.Address == other.Address && this.DepartmentName == other.DepartmentName;
			}
			return false;
		}

		/// <summary>
		/// Returns an integer suitable for hashing this object.
		/// </summary>
		public override int GetHashCode()
		{
			return -0x502B77AC ^ this.Name.GetHashCode() & this.Address.GetHashCode() ^ this.DepartmentName.GetHashCode();
		}

		/// <summary>
		/// Gets an empty institution.
		/// </summary>
		public static Institution Empty
		{
			get { return new Institution(); }
		}

		/// <summary>
		/// Gets the institution defined in the attributes of the <paramref name="dcm">DICOM file</paramref>.
		/// </summary>
		public static Institution GetInstitution(DicomFile dcm)
		{
			Institution institution = new Institution();
			GeneralEquipmentModuleIod iod = new GeneralEquipmentModuleIod(dcm.DataSet);
			institution.Name = iod.InstitutionName ?? string.Empty;
			institution.Address = iod.InstitutionAddress ?? string.Empty;
			institution.DepartmentName = iod.InstitutionalDepartmentName ?? string.Empty;
			return institution;
		}
	}
}
