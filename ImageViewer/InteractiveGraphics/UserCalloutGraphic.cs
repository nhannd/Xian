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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	//TODO (CR May09): no IPointsGraphic, add new ICalloutGraphic interface
	[Cloneable]
	public class UserCalloutGraphic : CalloutGraphic, ITextGraphic, IPointGraphic
	{
		private event EventHandler _pointChanged;
		private event EventHandler _textChanged;

		/// <summary>
		/// Instantiates a new instance of <see cref="UserCalloutGraphic"/>.
		/// </summary>
		public UserCalloutGraphic() : base("") {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected UserCalloutGraphic(UserCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets the text label.
		/// </summary>
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		string ITextGraphic.Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		public event EventHandler TextChanged
		{
			add { _textChanged += value; }
			remove { _textChanged -= value; }
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			EventsHelper.Fire(_textChanged, this, new EventArgs());
		}

		protected override void OnEndPointChanged()
		{
			base.OnEndPointChanged();

			EventsHelper.Fire(_pointChanged, this, new ListEventArgs<PointF>(this.EndPoint, 0));
		}

		protected new TextEditControlGraphic TextControlGraphic
		{
			get { return (TextEditControlGraphic) base.TextControlGraphic; }
		}

		protected override IControlGraphic InitializeTextControlGraphic(IGraphic textGraphic)
		{
			return new TextEditControlGraphic(new TextPlaceholderControlGraphic(base.InitializeTextControlGraphic(textGraphic)));
		}

		public bool StartEdit()
		{
			return this.TextControlGraphic.StartEdit();
		}

		public void EndEdit()
		{
			this.TextControlGraphic.EndEdit();
		}

		#region IPointGraphic Members

		PointF IPointGraphic.Point
		{
			get { return this.EndPoint; }
			set { this.EndPoint = value; }
		}

		event EventHandler IPointGraphic.PointChanged
		{
			add { _pointChanged += value; }
			remove { _pointChanged -= value; }
		}

		#endregion
	}
}