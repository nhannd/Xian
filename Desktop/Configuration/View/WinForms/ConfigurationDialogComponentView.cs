using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
	[ExtensionOf(typeof(ConfigurationDialogComponentViewExtensionPoint))]
	public class ConfigurationDialogComponentView : WinFormsView, IApplicationComponentView
	{
		private class NavigatorApplyHandler : INavigatorApplyHandler
		{
			private readonly ConfigurationDialogComponent _component;

			public NavigatorApplyHandler(ConfigurationDialogComponent component)
			{
				_component = component;
			}

			#region INavigatorApplyHandler Members

			public void Apply()
			{
				_component.Apply();
			}

			public bool ApplyEnabled
			{
				get { return _component.ApplyEnabled; }
			}

			public event EventHandler ApplyEnabledChanged
			{
				add { _component.ApplyEnabledChanged += value; }
				remove { _component.ApplyEnabledChanged -= value; }
			}

			#endregion
		}

		private ConfigurationDialogComponent _component;

		#region IApplicationComponentView Members

		public ConfigurationDialogComponentView()
		{
		}

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ConfigurationDialogComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				NavigatorApplyHandler applyHandler = null;
				if (_component.ApplyVisible)
					applyHandler = new NavigatorApplyHandler(_component);

				return new NavigatorComponentContainerControl(_component, applyHandler);
			}
		}
	}
}
