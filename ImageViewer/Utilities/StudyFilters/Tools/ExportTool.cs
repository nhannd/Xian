using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("export", DefaultToolbarActionSite + "/ToolbarAnonymizeExport", "Export")]
	[ButtonAction("saveto", DefaultToolbarActionSite + "/ToolbarSaveTo", "SaveTo")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public void Export()
		{
			// still no folder picker ><
			if (base.Selection.Count > 0)
			{
				string path = null;

				foreach (StudyItem item in base.Selection)
				{
					path = path ?? item.File.Directory.FullName;
				}
			}
		}

		public void SaveTo()
		{
			// no folder picker ><
			if (base.Selection.Count > 0)
			{
				string outputDir = FolderPickerExtensionPoint.GetFolder();
				int count = 0;

				foreach (StudyItem item in base.Selection)
				{
					FileInfo file = item.File;
					if (file.Exists)
					{
						string newpath = Path.Combine(outputDir, file.Name);
						if (!File.Exists(newpath))
						{
							file.CopyTo(newpath);
							count ++;
						}
					}
				}

				if (count > 0)
				{
					Process p = Process.Start(outputDir);
				}
			}
		}
	}
}