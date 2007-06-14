using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[ExtensionPoint]
	public class DropDownButtonActionViewExtensionPoint : ExtensionPoint<IActionView>
	{
	}

	[AssociateView(typeof(DropDownButtonActionViewExtensionPoint))]
	public class DropDownButtonAction : Action
	{
		private MenuModelDelegate _menuModelDelegate;

		/// <summary>
		/// Initializes a new instance of <see cref="DropDownButtonAction"/>.
		/// </summary>
		/// <param name="actionID"></param>
		/// <param name="path"></param>
		/// <param name="resourceResolver"></param>
		public DropDownButtonAction(string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{

		}

		/// <summary>
		/// Gets the menu model for the dropdown menu.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get { return _menuModelDelegate(); }
		}

		internal void SetMenuModelDelegate(MenuModelDelegate menuModelDelegate)
		{
			_menuModelDelegate = menuModelDelegate;
		}
	}
}
