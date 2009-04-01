using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms {
	[ExtensionOf(typeof(FolderPickerExtensionPoint))]
	public class FolderPicker : IFolderPicker {
		#region IFolderPicker Members

		public string GetFolder() {
			using(FolderBrowserDialog dlg = new FolderBrowserDialog())
			{
				if (dlg.ShowDialog() == DialogResult.OK)
					return dlg.SelectedPath;

				return null;
			}
		}

		#endregion
	}
}
