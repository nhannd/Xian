using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	[ExtensionOf(typeof(SimpleComposerAdapterComponentViewExtensionPoint))]
	public class SimpleComposerAdapterComponentView : WinFormsView, IApplicationComponentView {
		private SimpleComposerAdapterComponent _component;
		private SimpleComposerAdapterComponentPanel _control;

		public void SetComponent(IApplicationComponent component) {
			_component = (SimpleComposerAdapterComponent)component;
		}

		public override object GuiElement {
			get {
				if(_control == null)
				{
					_control = new SimpleComposerAdapterComponentPanel(_component);
				}
				return _control;
			}
		}
	}
}