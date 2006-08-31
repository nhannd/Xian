using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class AENavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponent : ApplicationComponent
	{
	}
}
