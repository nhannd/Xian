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

	public class InteractiveHitTestInfo
	{
		private ActiveZones _activeZone;
		private int _controlPoint;

		public InteractiveHitTestInfo()
		{
		}

		public ActiveZones ActiveZone
		{
			get
			{
				return _activeZone;
			}
			set
			{
				Platform.CheckForNullReference(value, "ActiveZone");
				_activeZone = value;
			}
		}

		public int ControlPoint
		{
			get
			{
				return _controlPoint;
			}
			set
			{
				_controlPoint = value;
			}
		}
	}
}
