using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="MonitorConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class MonitorConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public enum WindowBehaviour
	{
		Auto,
		Single,
		Separate
	}

	/// <summary>
	/// MonitorConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(MonitorConfigurationApplicationComponentViewExtensionPoint))]
	public class MonitorConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		private WindowBehaviour _windowBehaviour;

		/// <summary>
		/// Constructor
		/// </summary>
		public MonitorConfigurationApplicationComponent()
		{
		}

		public bool SingleWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Single;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Single;
					this.Modified = true;
				}
			}
		}

		public bool SeparateWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Separate;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Separate;
					this.Modified = true;
				}
			}
		}

		public override void Start()
		{
			_windowBehaviour = (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			MonitorConfigurationSettings.Default.WindowBehaviour = (int)_windowBehaviour;
			MonitorConfigurationSettings.Default.Save();
		}
	}
}