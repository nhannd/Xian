using System;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.Model.Tools;
using ClearCanvas.Workstation.Model.Actions;

namespace ClearCanvas.Workstation.Tools.Measurement
{
    [MenuAction("activate", "MenuTools/MenuToolsMeasurement/ToolsMeasurementEllipticalROI", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [ClickHandler("activate", "Select")]
    
    /// <summary>
	/// Summary description for EllipticalROITool.
	/// </summary>
	[ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
    public class EllipticalROI : MouseTool
	{
		public EllipticalROI()
            :base(XMouseButtons.Right, false)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return true;
		}

		public override bool OnKeyDown(XKeyEventArgs e)
		{
			return false;
		}

		public override bool OnKeyUp(XKeyEventArgs e)
		{
			return false;
		}

		#endregion

	}
}
