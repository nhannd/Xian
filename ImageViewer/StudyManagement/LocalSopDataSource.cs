using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class LocalSopDataSource : DicomMessageSopDataSource, ILocalSopDataSource
	{
		public LocalSopDataSource(string fileName)
			: base(new DicomFile(fileName), true)
		{
		}

		public LocalSopDataSource(DicomFile localFile)
			: base(localFile, true)
		{
		}

		#region ILocalSopDataSource Members

		public DicomFile File
		{
			get { return (DicomFile)SourceMessage; }
		}

		public string Filename
		{
			get { return File.Filename; }
		}

		protected override void EnsureLoaded()
		{
			File.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		#endregion
	}
}
