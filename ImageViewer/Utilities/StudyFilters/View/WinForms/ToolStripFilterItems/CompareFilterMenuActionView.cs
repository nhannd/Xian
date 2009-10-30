using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	[ExtensionOf(typeof (CompareFilterMenuActionViewExtensionPoint))]
	public class CompareFilterMenuActionView : WinFormsActionView
	{
		private object _guiElement;

		public CompareFilterMenuActionView()
		{}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
				{	
					_guiElement = new ClickableToolStripControlHost(
						new CompareFilterMenuActionControl((CompareFilterMenuAction)base.Context.Action));
				}

				return _guiElement;
			}
		}
	}
}