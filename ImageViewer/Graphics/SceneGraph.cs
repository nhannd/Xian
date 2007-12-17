#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// The root <see cref="CompositeGraphic"/> in the <see cref="PresentationImage"/>.
	/// </summary>
	internal class SceneGraph : CompositeGraphic
	{
		private event EventHandler<RectangleChangedEventArgs> _clientRectangleChangedEvent;

		internal SceneGraph()
		{
		}

		internal event EventHandler<RectangleChangedEventArgs> ClientRectangleChanged
		{
			add { _clientRectangleChangedEvent += value; }
			remove { _clientRectangleChangedEvent -= value; }
		}


		internal Rectangle ClientRectangle
		{
			get { return base.SpatialTransform.ClientRectangle; }
			set
			{
				SetClientRectangle(this, value);

				EventsHelper.Fire(_clientRectangleChangedEvent, this, new RectangleChangedEventArgs(value));
			}
		}

		private void SetClientRectangle(CompositeGraphic compositeGraphic, Rectangle clientRectangle)
		{
			compositeGraphic.SpatialTransform.ClientRectangle = clientRectangle;

			foreach (Graphic graphic in compositeGraphic.Graphics)
			{
				CompositeGraphic childGraphic = graphic as CompositeGraphic;

				if (childGraphic != null)
					SetClientRectangle(childGraphic, clientRectangle);
				else
					graphic.SpatialTransform.ClientRectangle = clientRectangle;
			}
		}
	}
}
