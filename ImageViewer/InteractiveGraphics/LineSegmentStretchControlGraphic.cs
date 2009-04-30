#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class LineSegmentStretchControlGraphic : ControlPointsGraphic, IMemorable
	{
		public LineSegmentStretchControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ILineSegmentGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Add(this.Subject.Pt1);
				base.ControlPoints.Add(this.Subject.Pt2);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		private LineSegmentStretchControlGraphic(LineSegmentStretchControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new ILineSegmentGraphic Subject
		{
			get { return base.Subject as ILineSegmentGraphic; }
		}

		public override string CommandName
		{
			get { return SR.CommandChange; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.Pt1Changed += OnSubjectPt1Changed;
			this.Subject.Pt2Changed += OnSubjectPt2Changed;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.Pt1Changed -= OnSubjectPt1Changed;
			this.Subject.Pt2Changed -= OnSubjectPt2Changed;
			base.Dispose(disposing);
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			PointsMemento pointsMemento = new PointsMemento();

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointsMemento.Add(this.Subject.Pt1);
				pointsMemento.Add(this.Subject.Pt2);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointsMemento;
		}

		public void SetMemento(object memento)
		{
			PointsMemento pointsMemento = memento as PointsMemento;
			if (pointsMemento == null || pointsMemento.Count != 2)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Pt1 = pointsMemento[0];
				this.Subject.Pt2 = pointsMemento[1];
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		#endregion

		private void OnSubjectPt1Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Pt1;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		private void OnSubjectPt2Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[1] = this.Subject.Pt2;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (index == 0)
				this.Subject.Pt1 = point;
			else if (index == 1)
				this.Subject.Pt2 = point;
			base.OnControlPointChanged(index, point);
		}
	}
}