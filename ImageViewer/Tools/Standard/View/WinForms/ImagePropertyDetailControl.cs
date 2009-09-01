using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using Application=ClearCanvas.Desktop.Application;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class ImagePropertyDetailControl : UserControl
	{
		public ImagePropertyDetailControl(string name, string description, string value)
		{
			InitializeComponent();

			_richText.Text = value;
			_name.Text = name;
			_description.Text = description;
		}
	}

	internal class DummyComponent : ApplicationComponent
	{
		public DummyComponent()
		{
		}
	}

	internal class CancelController : IButtonControl
	{
		readonly Form _parent;

		public CancelController(Form parent)
		{
			_parent = parent;
		}

		#region IButtonControl Members

		public DialogResult DialogResult
		{
			get
			{
				return System.Windows.Forms.DialogResult.Cancel;
			}
			set
			{
			}
		}

		public void NotifyDefault(bool value)
		{
		}

		public void PerformClick()
		{
			_parent.Close();
		}

		#endregion
	}

	internal class ShowValueDialog : DialogBox
	{
		private ShowValueDialog(string text)
			: base(CreateArgs(), Application.ActiveDesktopWindow)
        {
        }

		public static void Show(string name, string description, string text)
		{
			ShowValueDialog dialog = new ShowValueDialog(text);
			DialogBoxForm form = new DialogBoxForm(dialog, new ImagePropertyDetailControl(name, description, text));
			form.Text = SR.TitleDetails;
			form.CancelButton = new CancelController(form);
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = Cursor.Position - new Size(form.DesktopBounds.Width/2, form.DesktopBounds.Height/2);
			form.ShowDialog();
			form.Dispose();
		}

		private static DialogBoxCreationArgs CreateArgs()
		{
			DialogBoxCreationArgs args = new DialogBoxCreationArgs();
			args.Component = new DummyComponent();
			args.SizeHint = DialogSizeHint.Auto;
			return args;
		}
	}
}
