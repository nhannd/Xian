using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	public partial class ExternalPropertiesComponentControl : UserControl
	{
		public ExternalPropertiesComponentControl(ExternalPropertiesComponent component)
		{
			InitializeComponent();

			Control control = component.ExternalGuiElement;
			control.Dock = DockStyle.Fill;
			this.Size = control.Size;
			this.Controls.Add(control);
		}
	}
}