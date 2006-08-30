using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class DicomExplorerControl : UserControl
	{
		private DicomExplorerComponent _component;

		public DicomExplorerControl(DicomExplorerComponent component)
		{
			InitializeComponent();
		}
	}
}
