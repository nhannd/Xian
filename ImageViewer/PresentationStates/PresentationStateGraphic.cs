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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// The graphic at the root of the scene graph that handles deserializing of the image's presentation state.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="IPresentationImage"/>s can insert this graphic at the root of the scene graphic <i>before</i>
	/// the <see cref="ImageGraphic"/> and other <see cref="CompositeGraphic"/>s in order to give the
	/// presentation state a first chance to set up the display and other graphics further down the scene graph
	/// before it is drawn for the first time.
	/// </para>
	/// <para>
	/// The presentation state is only deserialized on the first draw after construction or after the
	/// <see cref="PresentationState"/> property changes.
	/// </para>
	/// </remarks>
	[Cloneable(true)]
	public class PresentationStateGraphic : Graphic
	{
		[CloneCopyReference]
		private PresentationState _presentationState = null;

		private bool _applied = false;

		/// <summary>
		/// Constructs a new presentation state graphic.
		/// </summary>
		public PresentationStateGraphic() : this(null) {}

		/// <summary>
		/// Constructs a new presentation state graphic with the given initial presentation state.
		/// </summary>
		/// <param name="presentationState">The initial presentation state.</param>
		public PresentationStateGraphic(PresentationState presentationState)
		{
			_presentationState = presentationState;
		}

		/// <summary>
		/// Gets or sets the presentation state.
		/// </summary>
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

		/// <summary>
		/// Gets a value indicating if the presentation state has been deserialized to the image yet.
		/// </summary>
		public bool Applied
		{
			get { return _applied; }
		}

		/// <summary>
		/// Resets the graphic so that the presentation state will be deserialized on the next <see cref="IDrawable.Draw"/>.
		/// </summary>
		public void Reset()
		{
			_applied = false;
		}

		/// <summary>
		/// This method doesn't apply to this class.
		/// </summary>
		public override bool HitTest(Point point)
		{
			return false;
		}

		/// <summary>
		/// This method doesn't apply to this class.
		/// </summary>
		public override void Move(SizeF delta)
		{
			return;
		}

		/// <summary>
		/// Called when the value of <see cref="PresentationState"/> changes.
		/// </summary>
		protected virtual void OnPresentationStateChanged()
		{
			this.Reset();
		}

		/// <summary>
		/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
		/// for each object just before it is drawn/rendered, hence the reason it is public.
		/// </summary>
		/// <remarks>
		/// When this graphic is drawn, the <see cref="PresentationState"/> is deserialized. Any exceptions are caught
		/// and drawn to an exception text graphic placed at the <see cref="IApplicationGraphicsProvider">application-level</see>,
		/// if available. If no such in-grahic exception reporting mechanism is available, then the exception is thrown and
		/// the renderer will handle it according to its specific implementation.
		/// </remarks>
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
				_textGraphic.Location = anchor;
				_textGraphic.ResetCoordinateSystem();
				base.OnDrawing();
			}
		}
	}
}