using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyObjects
{
	[MenuAction("activate", "clipboard-contextmenu/MenuCreateKeyObjectSelection", "CreateKeyObjectSelection")]
	[ButtonAction("activate", "clipboard-toolbar/TitleCreateKeyObjectSelection", "CreateKeyObjectSelection")]
	[Tooltip("activate", "TooltipCreateKeyObjectSelection")]
	[IconSet("activate", IconScheme.Colour, "Icons.CreateKeyObjectSelectionToolSmall.png", "Icons.CreateKeyObjectSelectionToolSmall.png", "Icons.CreateKeyObjectSelectionToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	//
	[ExtensionOf(typeof (ClipboardToolExtensionPoint))]
	internal class KeyObjectSelectionTool : ClipboardTool
	{
		public void CreateKeyObjectSelection()
		{
			Platform.CheckPositive(base.Context.ClipboardItems.Count, "Clipboard Item Count");

			KeyObjectSelectionEditorComponent component = new KeyObjectSelectionEditorComponent();
			foreach (IClipboardItem item in base.Context.ClipboardItems)
			{
				if (item.Item is IImageSet)
				{
					Enqueue(component, (IImageSet) item.Item);
				}
				else if (item.Item is IDisplaySet)
				{
					Enqueue(component, (IDisplaySet) item.Item);
				}
				else if (item.Item is IPresentationImage)
				{
					Enqueue(component, (IPresentationImage) item.Item);
				}
			}

			ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(base.Context.DesktopWindow, new SimpleComponentContainer(component), SR.TitleCreateKeyObjectSelection);
			if(result == ApplicationComponentExitCode.Accepted)
			{
				component.SaveToFile();
			}
		}

		public override void Initialize() {
			base.Initialize();
			base.Enabled = (base.Context.ClipboardItems.Count > 0);
		}

		protected override void OnClipboardItemsChanged() {
			base.OnClipboardItemsChanged();
			base.Enabled = (base.Context.ClipboardItems.Count > 0);
		}

		private static int Enqueue(KeyObjectSelectionEditorComponent component, IPresentationImage pImage)
		{
			IImageSopProvider sopProvider = pImage as IImageSopProvider;
			if (sopProvider == null)
				return 0;
			component.Images.Add(sopProvider.ImageSop);
			return 1;
		}

		private static int Enqueue(KeyObjectSelectionEditorComponent component, IDisplaySet dSet)
		{
			int count = 0;
			foreach (IPresentationImage pImage in dSet.PresentationImages)
			{
				count += Enqueue(component, pImage);
			}
			return count;
		}

		private static int Enqueue(KeyObjectSelectionEditorComponent component, IImageSet iSet)
		{
			int count = 0;
			foreach (IDisplaySet dSet in iSet.DisplaySets)
			{
				count += Enqueue(component, dSet);
			}
			return count;
		}
	}
}