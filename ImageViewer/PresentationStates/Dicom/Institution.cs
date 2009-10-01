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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents an institution's details.
	/// </summary>
	public struct Institution
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
