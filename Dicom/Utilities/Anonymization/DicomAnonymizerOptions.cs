using System;

namespace ClearCanvas.Dicom.Utilities.Anonymization {
	/// <summary>
	/// An enumeration of flags to control the behaviour of the <see cref="DicomAnonymizer"/> utility,
	/// such as validation checks in the anonymized data set.
	/// </summary>
	[FlagsAttribute]
	public enum DicomAnonymizerOptions : uint {

		#region Individual Flags - Study Level

		/// <summary>
		/// Indicates that the anonymizer should not enforce a non-empty patient ID in the anonymized data set.
		/// </summary>
		AllowEmptyPatientId = 0x01,

		/// <summary>
		/// Indicates that the anonymizer should not enforce a non-empty patient name in the anonymized data set.
		/// </summary>
		AllowEmptyPatientName = 0x02,

		/// <summary>
		/// Indicates that the anonymizer should not enforce a different patient's birthdate in the anonymized data set.
		/// </summary>
		AllowEqualBirthDate = 0x04,

		#endregion

		#region Group Flags

		/// <summary>
		/// Indicates that the anonymizer should relax all optional attribute value checks in the anonymized data set.
		/// </summary>
		RelaxAllChecks = AllowEmptyPatientId | AllowEmptyPatientName | AllowEqualBirthDate,

		/// <summary>
		/// Indicates that the anonymizer should use its default behaviour, which is to enforce non-empty and different values in all checked attributes.
		/// </summary>
		Default = 0x0

		#endregion
	}
}
