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

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class PolygonControlGraphic : VerticesControlGraphic
	{
		public PolygonControlGraphic(IGraphic subject)
			: base(subject)
		{
			ResyncEndPoints();
		}

		public PolygonControlGraphic(bool canAddRemoveVertices, IGraphic subject)
			: base(canAddRemoveVertices, subject)
		{
			ResyncEndPoints();
		}

		protected PolygonControlGraphic(PolygonControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void ResyncEndPoints()
		{
			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				pointsGraphic.Points[pointsGraphic.Points.Count - 1] = pointsGraphic.Points[0];
			}
		}

		protected override void InsertVertex()
		{
			base.InsertVertex();
			ResyncEndPoints();
		}

		protected override void DeleteVertex()
		{
			base.DeleteVertex();
			ResyncEndPoints();
		}

		public override void SetMemento(object memento)
		{
			base.SetMemento(memento);
			ResyncEndPoints();
		}

		protected override void OnSubjectPointChanged(object sender, IndexEventArgs e)
		{
			base.OnSubjectPointChanged(sender, e);

			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				if (e.Index == 0)
					base.OnSubjectPointChanged(sender, new IndexEventArgs(pointsGraphic.Points.Count - 1));
				if (e.Index == pointsGraphic.Points.Count - 1)
					base.OnSubjectPointChanged(sender, new IndexEventArgs(0));
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			base.OnControlPointChanged(index, point);

			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				if (index == 0)
					base.OnControlPointChanged(pointsGraphic.Points.Count - 1, point);
				if (index == pointsGraphic.Points.Count - 1)
					base.OnControlPointChanged(0, point);
			}
		}
	}
}