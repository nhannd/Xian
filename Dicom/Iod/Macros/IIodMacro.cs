namespace ClearCanvas.Dicom.Iod.Macros
{
	public interface IIodMacro
	{
		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		void InitializeAttributes();

		/// <summary>
		/// Gets the dicom attribute collection as a dicom sequence item.
		/// </summary>
		/// <value>The dicom sequence item.</value>
		DicomSequenceItem DicomSequenceItem { get; set; }
	}
}