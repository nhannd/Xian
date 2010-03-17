using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	[ExtensionOf(typeof(ComboBoxCellEditorViewExtensionPoint))]
	public class ComboBoxCellEditorView : WinFormsView, ITableCellEditorView
	{
		private readonly ComboBoxCellEditorControl _control;

		public ComboBoxCellEditorView()
		{
			_control = new ComboBoxCellEditorControl();
		}

		public void SetEditor(ITableCellEditor editor)
		{
			_control.SetEditor((ComboBoxCellEditor)editor);
		}

		public override object GuiElement
		{
			get { return _control; }
		}
	}}
