using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="MonitorConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionOf(typeof(MonitorConfigurationApplicationComponentViewExtensionPoint))]
	public class MonitorConfigurationApplicationComponentView : WinFormsView, IApplicationComponentView
	{
		private MonitorConfigurationApplicationComponent _component;
		private MonitorConfigurationApplicationComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MonitorConfigurationApplicationComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MonitorConfigurationApplicationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}