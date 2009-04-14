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