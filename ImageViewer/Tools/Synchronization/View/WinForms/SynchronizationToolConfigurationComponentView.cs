using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms
{
	[ExtensionOf(typeof(SynchronizationToolConfigurationComponentViewExtensionPoint))]
	public class SynchronizationToolConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private SynchronizationToolConfigurationComponent _component;
		private SynchronizationToolConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SynchronizationToolConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SynchronizationToolConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}