using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms
{
	[ExtensionOf(typeof (MprConfigurationComponentViewExtensionPoint))]
	public class MprConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private MprConfigurationComponent _component;
		private MprConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MprConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MprConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}