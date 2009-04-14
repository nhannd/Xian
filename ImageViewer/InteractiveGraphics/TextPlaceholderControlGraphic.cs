using System;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class TextPlaceholderControlGraphic : ControlPointsGraphic, IMemorable
	{
		public TextPlaceholderControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ITextGraphic));

			Initialize();
		}

		protected TextPlaceholderControlGraphic(TextPlaceholderControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.PropertyChanged += Subject_PropertyChanged;
		}

		public new ITextGraphic Subject
		{
			get { return base.Subject as ITextGraphic; }
		}

		public override string CommandName
		{
			get { return SR.CommandChange; }
		}

		#region IMemorable Members

		public virtual object CreateMemento()
		{
			PointMemento pointMemento;

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointMemento = new PointMemento(this.Subject.Location);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointMemento;
		}

		public virtual void SetMemento(object memento)
		{
			PointMemento pointMemento = memento as PointMemento;
			if (pointMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Location = pointMemento.Point;
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		#endregion

		private void Subject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Clear();
				if (string.IsNullOrEmpty(this.Subject.Text))
				{
					base.ControlPoints.Add(this.Subject.Location);
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			this.Subject.Location = point;
			this.Draw();
			base.OnControlPointChanged(index, point);
		}
	}
}