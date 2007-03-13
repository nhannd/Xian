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
		private string _name;
		private int _window;
		private int _level;

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowLevelPresetApplicationComponent(
			string name,
			int window,
			int level)
		{
			_name = name;
			_window = window;
			_level = level;
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
			// TODO prepare the component for its live phase
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}
	}
}
