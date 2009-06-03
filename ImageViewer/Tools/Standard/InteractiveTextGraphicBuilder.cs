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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Adds two events to mark completion or cancellation of the text graphic after the graphic builder loses
	/// mouse input capture and enters text edit mode (still part of text graphic building process).
	/// </summary>
	internal abstract class InteractiveTextGraphicBuilder : InteractiveGraphicBuilder
	{
		private event EventHandler<GraphicEventArgs> _graphicFinalComplete;
		private event EventHandler<GraphicEventArgs> _graphicFinalCancelled;

		private EditBox _currentCalloutEditBox;
		private ITextGraphic _textGraphic;

		protected InteractiveTextGraphicBuilder(IGraphic graphic) : base(graphic) {}

		public event EventHandler<GraphicEventArgs> GraphicFinalComplete
		{
			add { _graphicFinalComplete += value; }
			remove { _graphicFinalComplete -= value; }
		}

		public event EventHandler<GraphicEventArgs> GraphicFinalCancelled
		{
			add { _graphicFinalCancelled += value; }
			remove { _graphicFinalCancelled -= value; }
		}

		public override sealed void Reset()
		{
			throw new NotSupportedException();
		}

		protected override void Rollback()
		{
			throw new NotSupportedException();
		}

		protected override void NotifyGraphicComplete()
		{
			// Find the edit control graphic for the text graphic and invoke edit mode.
			IGraphic graphic = this.Graphic;
			while (graphic != null && !(graphic is TextEditControlGraphic) && !(graphic is UserCalloutGraphic))
				graphic = graphic.ParentGraphic;
			if (graphic is TextEditControlGraphic)
				_textGraphic = (TextEditControlGraphic) graphic;
			else if (graphic is UserCalloutGraphic)
				_textGraphic = (UserCalloutGraphic) graphic;

			base.NotifyGraphicComplete();
			this.StartEdit();
		}

		protected override void NotifyGraphicCancelled()
		{
			EventsHelper.Fire(_graphicFinalCancelled, this, new GraphicEventArgs(this.Graphic));
			base.NotifyGraphicCancelled();
		}

		/// <summary>
		/// Starts edit mode on the callout graphic by installing a <see cref="EditBox"/> on the
		/// <see cref="Tile"/> of the <see cref="Graphic.ParentPresentationImage">parent PresentationImage</see>.
		/// </summary>
		/// <returns>True if edit mode was successfully started; False otherwise.</returns>
		public bool StartEdit()
		{
			// remove any pre-existing edit boxes
			EndEdit();

			bool result = false;
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(_textGraphic.Text ?? string.Empty);
				if (string.IsNullOrEmpty(_textGraphic.Text))
					editBox.Value = SR.LabelEnterText;
				editBox.Multiline = true;
				editBox.Location = Point.Round(_textGraphic.Location);
				editBox.Size = Rectangle.Round(_textGraphic.BoundingBox).Size;
				editBox.FontName = _textGraphic.Font;
				editBox.FontSize = _textGraphic.SizeInPoints;
				editBox.ValueAccepted += OnEditBoxComplete;
				editBox.ValueCancelled += OnEditBoxComplete;
				InstallEditBox(_currentCalloutEditBox = editBox);
				result = true;
			}
			finally
			{
				this.Graphic.ResetCoordinateSystem();
			}

			return result;
		}

		/// <summary>
		/// Ends edit mode on the callout graphic if it is currently being edited. Has no effect otherwise.
		/// </summary>
		public void EndEdit()
		{
			if (_currentCalloutEditBox != null)
			{
				_currentCalloutEditBox.ValueAccepted -= OnEditBoxComplete;
				_currentCalloutEditBox.ValueCancelled -= OnEditBoxComplete;
				_currentCalloutEditBox = null;
			}
			InstallEditBox(null);
		}

		private void InstallEditBox(EditBox editBox)
		{
			if (this.Graphic.ParentPresentationImage != null)
			{
				if (this.Graphic.ParentPresentationImage.Tile != null)
					this.Graphic.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e)
		{
			bool cancelled = false;
			if (_currentCalloutEditBox != null)
			{
				cancelled = string.IsNullOrEmpty(_currentCalloutEditBox.LastAcceptedValue);
				if (!cancelled)
				{
					_textGraphic.Text = _currentCalloutEditBox.Value;
					_textGraphic.Draw();
				}
			}
			EndEdit();

			if (cancelled)
				EventsHelper.Fire(_graphicFinalCancelled, this, new GraphicEventArgs(this.Graphic));
			else
				EventsHelper.Fire(_graphicFinalComplete, this, new GraphicEventArgs(this.Graphic));
		}
	}
}