using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof (GalleryComponentViewExtensionPoint))]
	public class GalleryComponentView : WinFormsView, IApplicationComponentView
	{
		private GalleryComponent _component;
		private GalleryComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (GalleryComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new GalleryComponentControl(_component);
				}
				return _control;
			}
		}
	}
}