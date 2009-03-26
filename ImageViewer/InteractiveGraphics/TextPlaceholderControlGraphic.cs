using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class TextPlaceholderControlGraphic : ControlPointsGraphic
	{
		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

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

		private void Subject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_bypassControlPointChangedEvent = true;
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
				_bypassControlPointChangedEvent = false;
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				this.Subject.Location = point;
				this.Draw();
			}
			base.OnControlPointChanged(index, point);
		}
	}
}