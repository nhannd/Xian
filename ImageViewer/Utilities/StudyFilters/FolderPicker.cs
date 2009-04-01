using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public class FolderPickerExtensionPoint : ExtensionPoint<IFolderPicker>
	{
		public static string GetFolder()
		{
			FolderPickerExtensionPoint xp = new FolderPickerExtensionPoint();
			IFolderPicker picker = (IFolderPicker)xp.CreateExtension();
			return picker.GetFolder();
		}
	}

	public interface IFolderPicker
	{
		string GetFolder();
	}
}