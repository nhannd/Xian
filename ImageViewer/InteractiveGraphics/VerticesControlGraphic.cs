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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class VerticesControlGraphic : ControlPointsGraphic, IMemorable
	{
		private bool _canAddRemoveVertices = false;

		[CloneIgnore]
		private bool _suspendSubjectPointChangeEvents = false;

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
			this.Subject.PointChanged += OnSubjectPointChanged;
			this.Subject.PointsChanged += OnSubjectPointsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.PointChanged -= OnSubjectPointChanged;
			this.Subject.PointsChanged -= OnSubjectPointsChanged;
			base.Dispose(disposing);
		}

		#region Add/Remove Vertices

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

			object memento = this.CreateMemento();

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

			base.AddToCommandHistory(memento, this.CreateMemento());
		}

		protected virtual void DeleteVertex()
		{
			if (!_canAddRemoveVertices)
				return;

			object memento = this.CreateMemento();

			IPointsGraphic subject = this.Subject;
			if (subject.Points.Count > 1)
			{
				int index = this.HitTestControlPoint(Point.Round(_lastContextMenuPoint));
				if (index >= 0 && index < subject.Points.Count)
				{
					subject.Points.RemoveAt(index);
				}
			}

			base.AddToCommandHistory(memento, this.CreateMemento());
		}

		private void PerformInsertVertex()
		{
			this.InsertVertex();
			this.Draw();
		}

		private void PerformDeleteVertex()
		{
			this.DeleteVertex();
			this.Draw();
		}

		protected void OnCanAddRemoveVerticesChanged() {}

		protected override IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			_lastContextMenuPoint = mouseInformation.Location;

			if (!_canAddRemoveVertices)
				return null;

			if (!base.Subject.HitTest(Point.Round(_lastContextMenuPoint)))
				return null;

			int count = this.Subject.Points.Count;
			bool hit = base.ControlPoints.HitTest(Point.Round(_lastContextMenuPoint));

			IResourceResolver resolver = new ResourceResolver(this.GetType(), true);
			string @namespace = typeof (VerticesControlGraphic).FullName;

			MenuAction insertAction = new MenuAction(@namespace + ":insert", new ActionPath(site + "/MenuInsertVertex", resolver), ClickActionFlags.None, resolver);
			insertAction.GroupHint = new GroupHint("Tools.Graphics.Edit");
			insertAction.Label = SR.MenuInsertVertex;
			insertAction.Persistent = true;
			insertAction.SetClickHandler(this.PerformInsertVertex);

			MenuAction deleteAction = new MenuAction(@namespace + ":delete", new ActionPath(site + "/MenuDeleteVertex", resolver), ClickActionFlags.None, resolver);
			deleteAction.GroupHint = new GroupHint("Tools.Graphics.Edit");
			deleteAction.Label = SR.MenuDeleteVertex;
			deleteAction.Visible = hit && count > 1;
			deleteAction.Persistent = true;
			deleteAction.SetClickHandler(this.PerformDeleteVertex);

			return new ActionSet(new IAction[] {insertAction, deleteAction});
		}

		#endregion

		#region IMemorable Members

		public virtual object CreateMemento()
		{
			PointsMemento pointsMemento = new PointsMemento();

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				foreach (PointF point in this.Subject.Points)
					pointsMemento.Add(point);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointsMemento;
		}

		public virtual void SetMemento(object memento)
		{
			PointsMemento pointsMemento = memento as PointsMemento;
			if (pointsMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			_suspendSubjectPointChangeEvents = true;
			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				int numPoints = Math.Min(this.Subject.Points.Count, pointsMemento.Count);
				for (int n = 0; n < numPoints; n++)
					this.Subject.Points[n] = pointsMemento[n];
				for (int n = numPoints; n < this.Subject.Points.Count; n++)
					this.Subject.Points.RemoveAt(numPoints);
				for (int n = numPoints; n < pointsMemento.Count; n++)
					this.Subject.Points.Add(pointsMemento[n]);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
				_suspendSubjectPointChangeEvents = false;
				this.OnSubjectPointsChanged(this, new EventArgs());
			}
		}

		#endregion

		protected virtual void OnSubjectPointsChanged(object sender, EventArgs e)
		{
			if (_suspendSubjectPointChangeEvents)
				return;

			this.SuspendControlPointEvents();
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
				this.ResumeControlPointEvents();
			}
		}

		protected virtual void OnSubjectPointChanged(object sender, ListEventArgs<PointF> e)
		{
			if (_suspendSubjectPointChangeEvents)
				return;

			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[e.Index] = this.Subject.Points[e.Index];
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			this.Subject.Points[index] = point;
			base.OnControlPointChanged(index, point);
		}
	}
}