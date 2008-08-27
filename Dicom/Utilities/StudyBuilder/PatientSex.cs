namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// An enumeration representing the values of the Patient's Sex DICOM attribute (Tag 0010,0040).
	/// </summary>
	public enum PatientSex
	{
		/// <summary>
		/// Represents unrecognized and empty code strings.
		/// </summary>
		Undefined,

		/// <summary>
		/// Represents the M code string.
		/// </summary>
		Male,

		/// <summary>
		/// Represents the F code string.
		/// </summary>
		Female,

		/// <summary>
		/// Represents the O code string.
		/// </summary>
		Other
	}
}