using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms
{
	[ExtensionOf(typeof(SynchronizationToolConfigComponentViewExtensionPoint))]
	public class SynchronizationToolConfigComponentView : WinFormsView, IApplicationComponentView
	{
		private SynchronizationToolConfigComponent _component;
		private SynchronizationToolConfigComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SynchronizationToolConfigComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SynchronizationToolConfigComponentControl(_component);
				}
				return _control;
			}
		}
	}
}