using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	[ExtensionOf(typeof (ExternalPropertiesComponentViewExtensionPoint))]
	public class ExternalPropertiesComponentView : WinFormsView, IApplicationComponentView
	{
		private ExternalPropertiesComponent _component;
		private ExternalPropertiesComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ExternalPropertiesComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ExternalPropertiesComponentControl(_component);
				}
				return _control;
			}
		}
	}
}