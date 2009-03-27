using System;
using System.Collections.Generic;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	public class ConfigurationDialogComponent : NavigatorComponentContainer
	{
		private class NavigatorPagePathComparer : IComparer<NavigatorPage>
		{
			#region IComparer<NavigatorPage> Members

			/// <summary>
			/// Compares two <see cref="NavigatorPage"/>s.
			/// </summary>
			public int Compare(NavigatorPage x, NavigatorPage y)
			{
				if (x == null)
				{
					if (y == null)
						return 0;
					else
						return -1;
				}

				if (y == null)
					return 1;

				return x.Path.LocalizedPath.CompareTo(y.Path.LocalizedPath);
			}

			#endregion
		}

		private readonly int _initialPageIndex;
		private readonly ConfigurationPageManager _configurationPageManager;

		internal ConfigurationDialogComponent(string initialPagePath)
			: base(ConfigurationDialogSettings.Default.ShowApplyButton)
		{
			// We want to validate all configuration pages
			this.ValidationStrategy = new StartedComponentsValidationStrategy();

			_configurationPageManager = new ConfigurationPageManager();

			List<NavigatorPage> pages = new List<NavigatorPage>();

			foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
				pages.Add(new NavigatorPage(configurationPage.GetPath(), configurationPage.GetComponent()));

			pages.Sort(new NavigatorPagePathComparer());

			_initialPageIndex = 0;
			int i = 0;

			foreach (NavigatorPage page in pages)
			{
				//do the unresolved paths match?
				if (page.Path.ToString() == initialPagePath)
					_initialPageIndex = i;

				this.Pages.Add(page);
				++i;
			}

			if (Pages.Count == 0)
				throw new Exception(SR.MessageNoConfigurationPagesExist);
		}

		public IEnumerable<IConfigurationPage> ConfigurationPages
		{
			get { return _configurationPageManager.Pages; }
		}

		public override void Start()
		{
			base.Start();

			MoveTo(_initialPageIndex);
		}

		protected override void OnAccept()
		{
			Save();
		}

		protected override void OnApply()
		{
			Save();
		}

		private void Save()
		{
			try
			{
				foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
				{
					if (configurationPage.GetComponent().Modified)
						configurationPage.SaveConfiguration();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
					delegate()
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}
	}
}
