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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Interactive builder class that interprests two mouse clicks as the
	/// circle centre followed by a point on the circumference.
	/// </summary>
	/// <remarks>
	/// This builder takes exactly two clicks, after which the graphic is
	/// complete and control is released.
	/// </remarks>
	public class InteractiveCircleGraphicBuilder : InteractiveGraphicBuilder
	{
		private int _numberOfPointsAnchored = 0;
		private PointF _centre;

		/// <summary>
		/// Constructs an interactive builder for the specified boundable graphic.
		/// </summary>
		/// <param name="boundableGraphic">The boundable graphic to be interactively built.</param>
		public InteractiveCircleGraphicBuilder(IBoundableGraphic boundableGraphic) : base(boundableGraphic) {}

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IBoundableGraphic Graphic
		{
			get { return (IBoundableGraphic) base.Graphic; }
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public override void Reset()
		{
			_numberOfPointsAnchored = 0;
			base.Reset();
		}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected override void Rollback()
		{
			_numberOfPointsAnchored = Math.Max(_numberOfPointsAnchored - 1, 0);
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			// We just started creating
			if (_numberOfPointsAnchored == 0)
			{
				_centre = mouseInformation.Location;

				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.TopLeft = _centre;
				this.Graphic.BottomRight = _centre;
				this.Graphic.ResetCoordinateSystem();

				_numberOfPointsAnchored++;
			}
			// We're done creating
			else
			{
				_numberOfPointsAnchored++;
			
				this.NotifyGraphicComplete();
			}

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_numberOfPointsAnchored == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					float radius = (float) Vector.Distance(_centre, mouseInformation.Location);
					SizeF offset = new SizeF(radius, radius);
					this.Graphic.TopLeft = _centre - offset;
					this.Graphic.BottomRight = _centre + offset;
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}

				this.Graphic.Draw();
			}
			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}