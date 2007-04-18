using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="WindowLevelPresetApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class WindowLevelPresetApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WindowLevelPresetApplicationComponent class
	/// </summary>
	[AssociateView(typeof(WindowLevelPresetApplicationComponentViewExtensionPoint))]
	public class WindowLevelPresetApplicationComponent : ApplicationComponent
	{
		private readonly IList<XKeys> _availableKeys;
		private string _name;
		private int _window;
		private int _level;
		private XKeys _selectedKey;

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowLevelPresetApplicationComponent(
			IList<XKeys> availableKeys,
			XKeys selectedKey,
			string name,
			int window,
			int level)
		{
			_availableKeys = availableKeys;
			_selectedKey = selectedKey;
			_name = name;
			_window = window;
			_level = level;
		}

		public IEnumerable<XKeys> AvailableKeys
		{
			get { return _availableKeys; }
		}

		public XKeys SelectedKey
		{
			get { return _selectedKey; }
			set { _selectedKey = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int Window
		{
			get { return _window; }
			set { _window = value; }
		}

		public int Level
		{
			get { return _level; }
			set { _level = value; }
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}
	}
}
