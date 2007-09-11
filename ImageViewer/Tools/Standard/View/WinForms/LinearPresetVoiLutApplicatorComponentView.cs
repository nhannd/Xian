using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(LinearPresetVoiLutApplicatorComponentViewExtensionPoint))]
	public class LinearPresetVoiLutApplicatorComponentView : WinFormsView, IApplicationComponentView
	{
		private LinearPresetVoiLutApplicatorComponent _component;
		private LinearPresetVoiLutApplicatorComponentControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (LinearPresetVoiLutApplicatorComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new LinearPresetVoiLutApplicatorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
