using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Sop"/> objects.
	/// </summary>
	public class SopCollection : ObservableDictionary<string, Sop, SopEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopCollection"/>.
		/// </summary>
		public SopCollection()
		{

		}
	}
}
