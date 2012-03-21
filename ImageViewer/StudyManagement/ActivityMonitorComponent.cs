using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	[ExtensionPoint]
	public sealed class ActivityMonitorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(ActivityMonitorComponentViewExtensionPoint))]
	public class ActivityMonitorComponent : ApplicationComponent
	{
	}
}
