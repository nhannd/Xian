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