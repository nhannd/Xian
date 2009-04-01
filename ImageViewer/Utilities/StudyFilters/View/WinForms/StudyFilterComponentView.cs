using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[ExtensionOf(typeof (StudyFilterComponentViewExtensionPoint))]
	public class StudyFilterComponentView : WinFormsView, IApplicationComponentView
	{
		private StudyFilterComponent _component;
		private StudyFilterComponentPanel _control;

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new StudyFilterComponentPanel(_component);
				}
				return _control;
			}
		}

		public void SetComponent(IApplicationComponent component)
		{
			_component = (StudyFilterComponent) component;
		}
	}
}