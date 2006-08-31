using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	[ExtensionOf(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponentView : WinFormsView, IApplicationComponentView
	{
		private Control _control;
		private AENavigatorComponent _component;

		public AENavigatorComponentView()
		{

		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new AENavigatorControl(_component);
				}
				return _control;
			}
		}

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = component as AENavigatorComponent;
		}

		#endregion	
	}
}
