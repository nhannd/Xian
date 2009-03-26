using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class VerticesControlGraphic : ControlPointsGraphic
	{
		private bool _canAddRemoveVertices = false;

		[CloneIgnore]
		private ActionModelNode _actionModel;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		[CloneIgnore]
		private PointF _lastContextMenuPoint = PointF.Empty;

		public VerticesControlGraphic(IGraphic subject)
			: this(false, subject) {}

		public VerticesControlGraphic(bool canAddRemoveVertices, IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (IPointsGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				foreach(PointF point in this.Subject.Points)
				{
					base.ControlPoints.Add(point);
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			_canAddRemoveVertices = canAddRemoveVertices;

			Initialize();
		}

		protected VerticesControlGraphic(VerticesControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new IPointsGraphic Subject
		{
			get { return base.Subject as IPointsGraphic; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.PointChanged += OnSubjectPointChanged;
			this.Subject.PointsChanged += OnSubjectPointsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.PointChanged -= OnSubjectPointChanged;
			this.Subject.PointsChanged -= OnSubjectPointsChanged;
			base.Dispose(disposing);
		}

		public bool CanAddRemoveVertices
		{
			get { return _canAddRemoveVertices; }
			set
			{
				if(_canAddRemoveVertices!=value)
				{
					_canAddRemoveVertices = value;
					OnCanAddRemoveVerticesChanged();
				}
			}
		}

		protected virtual void InsertVertex()
		{
			if (!_canAddRemoveVertices)
				return;

			IPointsGraphic subject = this.Subject;
			subject.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				int index = this.HitTestControlPoint(Point.Round(_lastContextMenuPoint));

				if (index < 0)
				{
					// if inserting in middle of line, find which index to insert at
					index = subject.IndexOfNextPoint(_lastContextMenuPoint);
				}
				else if(index == subject.Points.Count - 1)
				{
					// if inserting on last point, append instead of inserting before
					index++;
				}

				if (index >= 0)
				{
					subject.Points.Insert(index, _lastContextMenuPoint);
				}
			}
			finally
			{
				subject.ResetCoordinateSystem();
			}

			subject.Draw();
		}

		protected virtual void DeleteVertex()
		{
			if (!_canAddRemoveVertices)
				return;

			IPointsGraphic subject = this.Subject;
			if (subject.Points.Count > 1)
			{
				int index = this.HitTestControlPoint(Point.Round(_lastContextMenuPoint));
				if (index >= 0 && index < subject.Points.Count)
				{
					subject.Points.RemoveAt(index);
				}
			}

			subject.Draw();
		}

		protected void OnCanAddRemoveVerticesChanged() {}

		protected override ActionModelNode OnGetContextMenuModel(IMouseInformation mouseInformation)
		{
			_lastContextMenuPoint = mouseInformation.Location;

			if (_actionModel == null)
			{
				MenuAction insertAction = new MenuAction("insert", new ActionPath("VerticesControlGraphic/InsertVertex", null), ClickActionFlags.None, null);
				insertAction.Label = SR.StringInsertVertex;
				insertAction.SetClickHandler(this.InsertVertex);

				MenuAction deleteAction = new MenuAction("delete", new ActionPath("VerticesControlGraphic/DeleteVertex", null), ClickActionFlags.None, null);
				deleteAction.Label = SR.StringDeleteVertex;
				deleteAction.SetClickHandler(this.DeleteVertex);

				ActionModelRoot model = new ActionModelRoot();
				model.InsertAction(insertAction);
				model.InsertAction(deleteAction);
				_actionModel = model;
			}

			int count = this.Subject.Points.Count;
			bool hit = base.ControlPoints.HitTest(Point.Round(_lastContextMenuPoint));
			foreach (MenuAction action in _actionModel.GetActionsInOrder())
			{
				if (action.ActionID == "insert")
					action.Visible = (true || count == 1);
				else if (action.ActionID == "delete")
					action.Visible = (hit && count > 1);
			}

			return _canAddRemoveVertices ? _actionModel : null;
		}

		protected virtual void OnSubjectPointsChanged(object sender, EventArgs e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// resync control points
				this.ControlPoints.Clear();
				foreach (PointF point in this.Subject.Points)
				{
					base.ControlPoints.Add(point);
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}

		protected virtual void OnSubjectPointChanged(object sender, ListEventArgs<PointF> e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[e.Index] = this.Subject.Points[e.Index];
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				this.Subject.Points[index] = point;
			}
			base.OnControlPointChanged(index, point);
		}
	}
}