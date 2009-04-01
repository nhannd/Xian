using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[ExtensionOf(typeof(FilterEditorComponentViewExtensionPoint))]
	internal class FilterEditorComponentView : WinFormsView, IApplicationComponentView
	{
		private FilterEditorComponent _component;
		private FilterEditorComponentPanel _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (FilterEditorComponent)component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new FilterEditorComponentPanel(_component);
				}
				return _control;
			}
		}
	}
}