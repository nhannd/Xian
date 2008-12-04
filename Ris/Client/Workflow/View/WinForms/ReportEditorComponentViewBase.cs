using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	public class ReportEditorComponentViewBase : WinFormsView, IApplicationComponentView
	{
		private IReportEditorComponent _component;
		private RichTextReportEditorComponentControl _control;

		#region IApplicationComponentView Members

		/// <summary>
		/// Called by the host to assign this view to a component.
		/// </summary>
		public void SetComponent(IApplicationComponent component)
		{
			_component = (IReportEditorComponent)component;
		}

		#endregion

		/// <summary>
		/// Gets the underlying GUI component for this view.
		/// </summary>
		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new RichTextReportEditorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}