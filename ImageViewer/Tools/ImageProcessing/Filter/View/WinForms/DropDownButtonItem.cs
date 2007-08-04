using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.View.WinForms
{
	public class DropDownButtonItem : ToolStripDropDownButton
	{
		private DropDownButtonAction _action;
		private EventHandler _actionEnabledChangedHandler;
		private EventHandler _actionVisibleChangedHandler;
		private EventHandler _actionLabelChangedHandler;
		private EventHandler _actionTooltipChangedHandler;

		public DropDownButtonItem(DropDownButtonAction action)
		{
			_action = action;

			_actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
			_actionVisibleChangedHandler = new EventHandler(OnActionVisibleChanged);
			_actionLabelChangedHandler = new EventHandler(OnActionLabelChanged);
			_actionTooltipChangedHandler = new EventHandler(OnActionTooltipChanged);

			_action.EnabledChanged += _actionEnabledChangedHandler;
			_action.VisibleChanged += _actionVisibleChangedHandler;
			_action.LabelChanged += _actionLabelChangedHandler;
			_action.TooltipChanged += _actionTooltipChangedHandler;

			this.Text = _action.Label;
			this.Enabled = _action.Enabled;
			this.Visible = _action.Visible;
			this.ToolTipText = _action.Tooltip;

			if (_action.IconSet != null && _action.ResourceResolver != null)
			{
				try
				{
					this.Image = IconFactory.CreateIcon(_action.IconSet.MediumIcon, _action.ResourceResolver);
				}
				catch (Exception e)
				{
					// the icon was either null or not found - log some helpful message
					Platform.Log(LogLevel.Error, e);
				}
			}

			this.ShowDropDownArrow = true;
			
			// Build the dropdown menu
			ToolStripDropDownMenu dropDownMenu = new ToolStripDropDownMenu();
			ToolStripBuilder.BuildMenu(dropDownMenu.Items, _action.DropDownMenuModel.ChildNodes);
			this.DropDown = dropDownMenu;
		}

		private void OnActionEnabledChanged(object sender, EventArgs e)
		{
			this.Enabled = _action.Enabled;
		}

		private void OnActionVisibleChanged(object sender, EventArgs e)
		{
			this.Visible = _action.Visible;
		}

		private void OnActionLabelChanged(object sender, EventArgs e)
		{
			this.Text = _action.Label;
		}

		private void OnActionTooltipChanged(object sender, EventArgs e)
		{
			this.ToolTipText = _action.Tooltip;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _action != null)
			{
				// VERY IMPORTANT: instances of this class will be created and discarded frequently
				// throughout the lifetime of the application
				// therefore is it extremely important that the event handlers are disconnected
				// from the underlying _action events
				// otherwise, this object will hang around for the entire lifetime of the _action object,
				// even though this object is no longer needed
				_action.EnabledChanged -= _actionEnabledChangedHandler;
				_action.VisibleChanged -= _actionVisibleChangedHandler;
				_action.LabelChanged -= _actionLabelChangedHandler;
				_action.TooltipChanged -= _actionTooltipChangedHandler;

				_action = null;
			}
			base.Dispose(disposing);
		}
	}
}
