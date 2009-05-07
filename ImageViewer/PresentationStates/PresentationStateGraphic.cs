using System;
using System.Drawing;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[Cloneable(true)]
	public class PresentationStateGraphic : Graphic
	{
		[CloneCopyReference]
		private PresentationState _presentationState = null;

		private bool _applied = false;

		public PresentationStateGraphic() : this(null) {}

		public PresentationStateGraphic(PresentationState presentationState)
		{
			_presentationState = presentationState;
		}

		public PresentationState PresentationState
		{
			get { return _presentationState; }
			set
			{
				if (_presentationState != value)
				{
					_presentationState = value;
					this.OnPresentationStateChanged();
				}
			}
		}

		public bool Applied
		{
			get { return _applied; }
		}

		public void Reset()
		{
			_applied = false;
		}

		public override bool HitTest(Point point)
		{
			return false;
		}

		public override void Move(SizeF delta)
		{
			return;
		}

		protected virtual void OnPresentationStateChanged()
		{
			this.Reset();
		}

		public override void OnDrawing()
		{
			if (!_applied && base.ParentPresentationImage != null)
			{
				// set flag up here, in case deserializing the presentation state causes another draw
				_applied = true;
				if (_presentationState != null)
				{
					Exception exception = null;
					try
					{
						_presentationState.Deserialize(base.ParentPresentationImage);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Warn, ex, "An error has occurred while deserializing the image presentation state.");
						exception = ex;
					}

					if (base.ParentPresentationImage is IApplicationGraphicsProvider)
					{
						ExceptionGraphic exGraphic = (ExceptionGraphic) CollectionUtils.SelectFirst(
							((IApplicationGraphicsProvider) base.ParentPresentationImage).ApplicationGraphics, IsType<ExceptionGraphic>);
						if (exGraphic == null)
							((IApplicationGraphicsProvider) base.ParentPresentationImage).ApplicationGraphics.Add(exGraphic = new ExceptionGraphic());
						exGraphic.Set(exception);
					}
					else if (exception != null)
					{
						// fallback mechanism when no other exception reporting mechanism is available
						throw exception;
					}
				}
			}

			base.OnDrawing();
		}

		private static bool IsType<T> (object test)
		{
			return test is T;
		}

		[Cloneable]
		private class ExceptionGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private InvariantTextPrimitive _textGraphic;

			public ExceptionGraphic() : base()
			{
				base.Graphics.Add(_textGraphic = new InvariantTextPrimitive());
				_textGraphic.Color = Color.WhiteSmoke;
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private ExceptionGraphic(ExceptionGraphic source, ICloningContext context) : base()
			{
				context.CloneFields(source, this);
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_textGraphic = (InvariantTextPrimitive)CollectionUtils.SelectFirst(base.Graphics, IsType<InvariantTextPrimitive>);
			}

			public void Set(Exception exception)
			{
				if (exception == null)
				{
					_textGraphic.Text = string.Empty;
					return;
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine(SR.MessagePresentationStateDeserializeFailure);
				sb.AppendLine(string.Format(SR.FormatExceptionReason, exception.Message));
				_textGraphic.Text = sb.ToString();
			}

			public override void OnDrawing()
			{
				// upon drawing, re-centre the text
				RectangleF bounds = base.ParentPresentationImage.ClientRectangle;
				PointF anchor = new PointF(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
				_textGraphic.CoordinateSystem = CoordinateSystem.Destination;
				_textGraphic.AnchorPoint = anchor;
				_textGraphic.ResetCoordinateSystem();
				base.OnDrawing();
			}
		}
	}
}