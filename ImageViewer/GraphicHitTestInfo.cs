using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public enum ActiveZones
	{
		ControlPoint = 0,
		Body = 1,
		None = 2
	}

	/// <summary>
	/// Summary description for HitTestInfo.
	/// </summary>
	public class InteractiveHitTestInfo
	{
		private ActiveZones _ActiveZone;
		private int _ControlPoint;

		public InteractiveHitTestInfo()
		{
		}

		public ActiveZones ActiveZone
		{
			get
			{
				return _ActiveZone;
			}
			set
			{
				Platform.CheckForNullReference(value, "ActiveZone");
				_ActiveZone = value;
			}
		}

		public int ControlPoint
		{
			get
			{
				return _ControlPoint;
			}
			set
			{
				_ControlPoint = value;
			}
		}
	}
}
