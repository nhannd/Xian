using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	[ExtensionOf(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponentView : WinFormsView, IApplicationComponentView
	{
		private Control _control;
		private StudyBrowserComponent _component;

		public StudyBrowserComponentView()
		{

		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new StudyBrowserControl(_component);
				}
				return _control;
			}
		}

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = component as StudyBrowserComponent;
		}

		#endregion	
	}
}
