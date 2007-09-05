using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="VoiLutConfigurationComponent"/>
	/// </summary>
	[ExtensionOf(typeof(EditPresetVoiLutLinearComponentViewExtensionPoint))]
	public class EditPresetVoiLutLinearComponentView : WinFormsView, IApplicationComponentView
	{
		private EditPresetVoiLutLinearComponent _component;
		private EditPresetVoiLutLinearComponentControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (EditPresetVoiLutLinearComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new EditPresetVoiLutLinearComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
