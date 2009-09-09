using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	[ExtensionPoint]
	public sealed class ExternalPropertiesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ExternalPropertiesComponentViewExtensionPoint))]
	public class ExternalPropertiesComponent : ApplicationComponent
	{
		private IExternal _external;
		private IExternalPropertiesView _externalCofigurationView;
		private string _initialState;

		public ExternalPropertiesComponent(IExternal external)
		{
			Platform.CheckForNullReference(external, "external");
			_external = external;
		}

		public IExternal External
		{
			get { return _external; }
		}

		public Control ExternalGuiElement
		{
			get
			{
				if (_externalCofigurationView == null)
				{
					_externalCofigurationView = (IExternalPropertiesView) ViewFactory.CreateAssociatedView(_external.GetType());
					_externalCofigurationView.SetExternalLauncher(_external);
				}
				return (Control) _externalCofigurationView.GuiElement;
			}
		}

		public override void Start()
		{
			base.Start();

			_initialState = _external.GetState();
		}

		public void ResetExternal()
		{
			_external.SetState(_initialState);
		}
	}
}