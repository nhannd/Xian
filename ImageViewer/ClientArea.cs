#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer
{
	// TODO: Get rid of this class
	public class ClientArea 
	{
		// Private attributes
		private Rectangle _clientRectangle = new Rectangle(0, 0, 0, 0);
		private Rectangle _parentRectangle = new Rectangle(0, 0, 0, 0);
		private RectangleF _normalizedRectangle = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

		// Constructor
		public ClientArea()
		{
		}

		// Properties
		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set
			{
				/*if (value.Left < 0 || value.Right < 0 ||
					value.Top < 0 || value.Bottom < 0 ||
					value.Left > value.Right ||
					value.Top > value.Bottom)
				{
					throw new ArgumentException(SR.ExceptionInvalidParentRectangle(value.Top, value.Left, value.Right, value.Bottom));
				}*/

				_parentRectangle = value;
				CalculateClientRectangle();
			}
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set
			{
				RectangleUtilities.VerifyNormalizedRectangle(value);
				_normalizedRectangle = value;
				CalculateClientRectangle();
			}
		}

		private void CalculateClientRectangle()
		{
			// Calculate client rectangle
			int left = _parentRectangle.Left + (int) (_normalizedRectangle.Left * _parentRectangle.Width);
			int right = _parentRectangle.Left + (int) (_normalizedRectangle.Right * _parentRectangle.Width);
			int top = _parentRectangle.Top + (int) (_normalizedRectangle.Top * _parentRectangle.Height);
			int bottom = _parentRectangle.Top + (int) (_normalizedRectangle.Bottom * _parentRectangle.Height);

			_clientRectangle = new Rectangle(left, top, right - left, bottom - top);
		}
	}
}
