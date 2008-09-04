using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	[ExtensionOf(typeof (StudyComposerItemEditorComponentViewExtensionPoint))]
	public class StudyComposerItemPropertiesComponentView : WinFormsView, IApplicationComponentView
	{
		private StudyComposerItemEditorComponent _component;
		private StudyComposerItemEditorComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (StudyComposerItemEditorComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new StudyComposerItemEditorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}