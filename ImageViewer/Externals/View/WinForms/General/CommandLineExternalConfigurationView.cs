using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	[ExtensionOf(typeof (CommandLineExternalConfigurationViewExtensionPoint))]
	public class CommandLineExternalConfigurationView : WinFormsView, IExternalPropertiesView
	{
		private CommandLineExternal _component;
		private CommandLineExternalConfigurationControl _control;

		public void SetExternalLauncher(IExternal component)
		{
			_component = (CommandLineExternal) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new CommandLineExternalConfigurationControl(_component);
				}
				return _control;
			}
		}
	}
}