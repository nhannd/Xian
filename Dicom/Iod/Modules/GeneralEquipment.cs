#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralEquipment Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.5.1 (Table C.7-8)</remarks>
	public class GeneralEquipmentModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
		/// </summary>	
		public GeneralEquipmentModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
		/// </summary>
		public GeneralEquipmentModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets or sets the value of Manufacturer in the underlying collection.
		/// </summary>
		public string Manufacturer
		{
			get { return base.DicomAttributeProvider[DicomTags.Manufacturer].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.Manufacturer].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of InstitutionName in the underlying collection.
		/// </summary>
		public string InstitutionName
		{
			get { return base.DicomAttributeProvider[DicomTags.InstitutionName].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.InstitutionName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of InstitutionAddress in the underlying collection.
		/// </summary>
		public string InstitutionAddress
		{
			get { return base.DicomAttributeProvider[DicomTags.InstitutionAddress].ToString(); }
			set { base.DicomAttributeProvider[DicomTags.InstitutionAddress].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the value of StationName in the underlying collection.
		/// </summary>
		public string StationName
		{
			get { return base.DicomAttributeProvider[DicomTags.StationName].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.StationName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of InstitutionalDepartmentName in the underlying collection.
		/// </summary>
		public string InstitutionalDepartmentName
		{
			get { return base.DicomAttributeProvider[DicomTags.InstitutionalDepartmentName].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.InstitutionalDepartmentName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ManufacturersModelName in the underlying collection.
		/// </summary>
		public string ManufacturersModelName
		{
			get { return base.DicomAttributeProvider[DicomTags.ManufacturersModelName].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ManufacturersModelName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DeviceSerialNumber in the underlying collection.
		/// </summary>
		public string DeviceSerialNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.DeviceSerialNumber].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.DeviceSerialNumber].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of SoftwareVersions in the underlying collection.
		/// </summary>
		public string SoftwareVersions
		{
			get { return base.DicomAttributeProvider[DicomTags.SoftwareVersions].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.SoftwareVersions].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of GantryId in the underlying collection.
		/// </summary>
		public string GantryId
		{
			get { return base.DicomAttributeProvider[DicomTags.GantryId].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.GantryId].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of SpatialResolution in the underlying collection.
		/// </summary>
		public string SpatialResolution
		{
			get { return base.DicomAttributeProvider[DicomTags.SpatialResolution].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.SpatialResolution].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DateOfLastCalibration and TimeOfLastCalibration in the underlying collection.
		/// </summary>
		public DateTime? DateTimeOfLastCalibrationDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.DateOfLastCalibration].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.TimeOfLastCalibration].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.DateOfLastCalibration];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.TimeOfLastCalibration];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PixelPaddingValue in the underlying collection.
		/// </summary>
		public int PixelPaddingValue
		{
			get { return base.DicomAttributeProvider[DicomTags.PixelPaddingValue].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.PixelPaddingValue].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.DateOfLastCalibration;
				yield return DicomTags.TimeOfLastCalibration;
				yield return DicomTags.DeviceSerialNumber;
				yield return DicomTags.GantryId;
				yield return DicomTags.InstitutionAddress;
				yield return DicomTags.InstitutionalDepartmentName;
				yield return DicomTags.InstitutionName;
				yield return DicomTags.Manufacturer;
				yield return DicomTags.ManufacturersModelName;
				yield return DicomTags.PixelPaddingValue;
				yield return DicomTags.SoftwareVersions;
				yield return DicomTags.SpatialResolution;
				yield return DicomTags.StationName;
			}
		}
	}
}