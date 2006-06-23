using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.DynamicOverlays
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
		private ActiveZones m_ActiveZone;
		private int m_ControlPoint;

		public InteractiveHitTestInfo()
		{
		}

		public ActiveZones ActiveZone
		{
			get
			{
				return m_ActiveZone;
			}
			set
			{
				Platform.CheckForNullReference(value, "ActiveZone");
				m_ActiveZone = value;
			}
		}

		public int ControlPoint
		{
			get
			{
				return m_ControlPoint;
			}
			set
			{
				m_ControlPoint = value;
			}
		}
	}
}
