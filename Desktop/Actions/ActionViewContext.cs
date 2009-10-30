using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	public interface IActionViewContext
	{
		IAction Action { get; }
		IconSize IconSize { get; set; }
		event EventHandler IconSizeChanged;
	}

	public class ActionViewContext : IActionViewContext
	{
		private readonly IAction _action;
		private IconSize _iconSize;
		private event EventHandler _iconSizeChanged;

		public ActionViewContext(IAction action)
			: this(action, default(IconSize))
		{
		}

		public ActionViewContext(IAction action, IconSize iconSize)
		{
			Platform.CheckForNullReference(action, "action");
			_action = action;
			_iconSize = iconSize;
		}

		#region IActionViewContext Members

		public IAction Action
		{
			get { return _action; }
		}

		public IconSize IconSize
		{
			get
			{
				return _iconSize;
			}
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					EventsHelper.Fire(_iconSizeChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler IconSizeChanged
		{
			add { _iconSizeChanged += value; }
			remove { _iconSizeChanged -= value; }
		}

		#endregion
	}
}