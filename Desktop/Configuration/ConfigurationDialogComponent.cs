using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	public sealed class ConfigurationDialogComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{}

	[AssociateView(typeof(ConfigurationDialogComponentViewExtensionPoint))]
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

		private bool _applyEnabled = false;
		private event EventHandler _applyEnabledChanged;

		private readonly int _initialPageIndex;
		private readonly ConfigurationPageManager _configurationPageManager;

		internal ConfigurationDialogComponent(string initialPagePath)
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

		public override void Accept()
		{
			Apply(true);
		}

		public bool ApplyVisible
		{
			get { return ConfigurationDialogSettings.Default.ShowApplyButton; }
		}

		public bool ApplyEnabled
		{
			get { return _applyEnabled; }
			protected set
			{
				if (_applyEnabled != value)
				{
					_applyEnabled = value;
					EventsHelper.Fire(_applyEnabledChanged, this, new EventArgs());
				}
			}
		}

		public event EventHandler ApplyEnabledChanged
		{
			add { _applyEnabledChanged += value; }
			remove { _applyEnabledChanged -= value; }
		}

		public virtual void Apply()
		{
			Apply(false);
		}

		protected override void OnComponentModifiedChanged(IApplicationComponent component)
		{
			base.OnComponentModifiedChanged(component);
			this.ApplyEnabled = this.Modified;
		}

		private void Apply(bool exit)
		{
			if (this.HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				try
				{
					foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
					{
						if (configurationPage.GetComponent().Modified)
							configurationPage.SaveConfiguration();
					}

					ApplyEnabled = false;
					if (exit)
						this.Exit(ApplicationComponentExitCode.Accepted);
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
}
