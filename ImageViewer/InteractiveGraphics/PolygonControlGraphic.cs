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

		protected override void OnSubjectPointChanged(object sender, ListEventArgs<PointF> e)
		{
			base.OnSubjectPointChanged(sender, e);

			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				if (e.Index == 0)
					base.OnSubjectPointChanged(sender, new ListEventArgs<PointF>(e.Item, pointsGraphic.Points.Count - 1));
				if (e.Index == pointsGraphic.Points.Count - 1)
					base.OnSubjectPointChanged(sender, new ListEventArgs<PointF>(e.Item, 0));
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