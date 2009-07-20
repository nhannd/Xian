using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions
{
	[ExtensionPoint]
	public class CompareFilterMenuActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (CompareFilterMenuActionViewExtensionPoint))]
	public class CompareFilterMenuAction : Action
	{
		private readonly int _allowedModesMask;
		private event EventHandler _currentModeChanged;
		private event EventHandler _valueChanged;
		private event EventHandler _refreshRequested;

		private CompareFilterMode _currentMode;
		private string _value;

		public CompareFilterMenuAction(string actionID, ActionPath actionPath, CompareFilterMode allowedModes, IResourceResolver resourceResolver)
			: base(actionID, actionPath, resourceResolver)
		{
			Platform.CheckTrue(allowedModes != 0, "allowedModes should be non-empty");
			_allowedModesMask = (int)allowedModes;

			foreach (CompareFilterMode mode in this.AllowedModes)
			{
				_currentMode = mode;
				break;
			}
		}

		public CompareFilterMode CurrentMode
		{
			get { return _currentMode; }
			set
			{
				value = (CompareFilterMode) ((int) value & _allowedModesMask);
				Platform.CheckTrue(value != 0, "That mode is not allowed");
				if (_currentMode != value)
				{
					_currentMode = value;
					EventsHelper.Fire(_currentModeChanged, this, EventArgs.Empty);
				}
			}
		}

		public string Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CurrentModeChanged
		{
			add { _currentModeChanged += value; }
			remove { _currentModeChanged -= value; }
		}

		public event EventHandler ValueChanged
		{
			add { _valueChanged += value; }
			remove { _valueChanged -= value; }
		}

		public event EventHandler RefreshRequested
		{
			add { _refreshRequested += value; }
			remove { _refreshRequested -= value; }
		}

		public void Refresh()
		{
			//EventsHelper.Fire(_refreshRequested, this, EventArgs.Empty);
		}

		public void ToggleMode( )
		{
			List<CompareFilterMode> list = new List<CompareFilterMode>(this.AllowedModes);
			int index = (Math.Max(-1, list.IndexOf(this.CurrentMode)) + 1) % list.Count;
			this.CurrentMode = list[index];
		}

		public IEnumerable<CompareFilterMode> AllowedModes
		{
			get
			{
				foreach (int mode in Enum.GetValues(typeof(CompareFilterMode)))
				{
					if((_allowedModesMask & mode) == mode)
						yield return (CompareFilterMode) mode;
				}
			}
		}

		public static CompareFilterMenuAction CreateAction(Type callingType, string actionID, string actionPath, CompareFilterMode allowedModes, IResourceResolver resourceResolver)
		{
			CompareFilterMenuAction action = new CompareFilterMenuAction(
				string.Format("{0}:{1}", callingType.FullName, actionID),
				new ActionPath(actionPath, resourceResolver),
				allowedModes, resourceResolver);
			action.Label = action.Path.LastSegment.LocalizedText;
			action.Persistent = true;
			return action;
		}
	}

	public enum CompareFilterMode : int
	{
		Equals = 0x1,
		NotEquals = 0x2,
		LessThan = 0x4,
		LessThenOrEquals = 0x8,
		GreaterThan = 0x10,
		GreaterThanOrEquals = 0x20
	}
}