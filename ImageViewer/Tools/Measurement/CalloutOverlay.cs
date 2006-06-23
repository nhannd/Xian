using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Workstation.Model;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// Summary description for RulerOverlay.
	/// </summary>
	public class CalloutGraphic : Graphic
	{
		LinePrimitive m_Line;

		public CalloutGraphic()
		{
		}

		public PointF Pt1
		{
			get
			{
				return m_Line.Pt1;
			}
			set
			{
				m_Line.Pt1 = value;
			}
		}

		public PointF Pt2
		{
			get
			{
				return m_Line.Pt2;
			}
			set
			{
				m_Line.Pt2 = value;
			}
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			return false;
		}


/*		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}*/
	}
}
