using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	[ExtensionOf(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponentView : WinFormsView, IApplicationComponentView
	{
		private Control _control;
		private LocalImageExplorerComponent _component;

		public LocalImageExplorerComponentView()
		{

		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new LocalImageExplorerControl(_component);
				}
				return _control;
			}
		}

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = component as LocalImageExplorerComponent;
		}

		#endregion

	}
}
