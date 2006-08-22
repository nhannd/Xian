using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// DynamicActionMouseTools are mousetools that require mouse capture on mouse down,
	/// and release the mouse capture on mouse up.  Any subclass of this class must call
	/// both the OnMouseDown and OnMouseUp methods.
	/// </summary>
	public class DynamicActionMouseTool : MouseTool
	{
        public DynamicActionMouseTool(XMouseButtons mouseButton, bool initiallyActive)
			: base(mouseButton, initiallyActive)
		{
        }

        public DynamicActionMouseTool(XMouseButtons mouseButton)
            : base(mouseButton, false)
        {
        }

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			if (e.MouseCapture != null)
				e.MouseCapture.SetCapture(this, e);

			return base.OnMouseDown(e);
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			bool returnValue = base.OnMouseUp(e); 
			
			if (e.MouseCapture != null)
				e.MouseCapture.ReleaseCapture();

			return returnValue;
		}
	}
}
