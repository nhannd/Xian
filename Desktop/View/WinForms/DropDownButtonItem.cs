using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class DropDownButtonItem : ToolStripDropDownButton
	{
		private IDropDownButtonAction _action;
		private EventHandler _actionEnabledChangedHandler;
		private EventHandler _actionVisibleChangedHandler;
		private EventHandler _actionLabelChangedHandler;
		private EventHandler _actionTooltipChangedHandler;
		private EventHandler _actionIconSetChangedHandler;

		public DropDownButtonItem(IDropDownButtonAction action)
		{
			_action = action;

			_actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
			_actionVisibleChangedHandler = new EventHandler(OnActionVisibleChanged);
			_actionLabelChangedHandler = new EventHandler(OnActionLabelChanged);
			_actionTooltipChangedHandler = new EventHandler(OnActionTooltipChanged);
			_actionIconSetChangedHandler = new EventHandler(OnActionIconSetChanged);

			_action.EnabledChanged += _actionEnabledChangedHandler;
			_action.VisibleChanged += _actionVisibleChangedHandler;
			_action.LabelChanged += _actionLabelChangedHandler;
			_action.TooltipChanged += _actionTooltipChangedHandler;
			_action.IconSetChanged += _actionIconSetChangedHandler;

			this.Text = _action.Label;
			this.Enabled = _action.Enabled;
			this.Visible = _action.Visible;
			this.ToolTipText = _action.Tooltip;

			UpdateVisibility();
			UpdateEnablement();
			UpdateIcon();

			this.ShowDropDownArrow = true;

			// Build the dropdown menu
			ToolStripDropDownMenu dropDownMenu = new ToolStripDropDownMenu();
			ToolStripBuilder.BuildMenu(dropDownMenu.Items, _action.DropDownMenuModel.ChildNodes);
			this.DropDown = dropDownMenu;
		}

		private void OnActionEnabledChanged(object sender, EventArgs e)
		{
			UpdateEnablement();
		}

		private void OnActionVisibleChanged(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		private void OnActionLabelChanged(object sender, EventArgs e)
		{
			this.Text = _action.Label;
		}

		private void OnActionTooltipChanged(object sender, EventArgs e)
		{
			this.ToolTipText = _action.Tooltip;
		}

		private void OnActionIconSetChanged(object sender, EventArgs e)
		{
			UpdateIcon();
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
				_action.IconSetChanged -= _actionIconSetChangedHandler;

				_action = null;
			}
			base.Dispose(disposing);
		}

		private void UpdateVisibility()
		{
			this.Visible = _action.Visible && (_action.Permissible || DesktopViewSettings.Default.ShowNonPermissibleActions);
		}

		private void UpdateEnablement()
		{
			this.Enabled = _action.Enabled && (_action.Permissible || DesktopViewSettings.Default.EnableNonPermissibleActions);
		}

		private void UpdateIcon()
		{
			if (_action.IconSet != null && _action.ResourceResolver != null)
			{
				try
				{
					Image oldImage = this.Image;
					this.Image = IconFactory.CreateIcon(_action.IconSet.MediumIcon, _action.ResourceResolver);

					if (oldImage != null)
						oldImage.Dispose();

					this.Invalidate();
				}
				catch (Exception e)
				{
					// the icon was either null or not found - log some helpful message
					Platform.Log(LogLevel.Error, e);
				}
			}
		}
	}
}
