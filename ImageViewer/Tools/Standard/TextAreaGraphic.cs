using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[Cloneable]
	internal class TextAreaGraphic : AnnotationGraphic
	{
		public TextAreaGraphic() : base(new UserTextGraphic()) {}

		protected TextAreaGraphic(TextAreaGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public UserTextGraphic UserTextGraphic
		{
			get { return (UserTextGraphic)base.InteractiveGraphic; }	
		}

		public PointF Location
		{
			get { return UserTextGraphic.Location; }
			set { UserTextGraphic.Location = value; }
		}

		public bool StartEdit()
		{
			return UserTextGraphic.StartEdit();
		}

		public void EndEdit()
		{
			UserTextGraphic.EndEdit();
		}

		public override GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedTextAreaGraphicState(this);
		}

		protected class FocussedSelectedTextAreaGraphicState : FocussedSelectedInteractiveGraphicState
		{
			public FocussedSelectedTextAreaGraphicState(TextAreaGraphic textAreaGraphic) : base(textAreaGraphic) {}

			protected new TextAreaGraphic StatefulGraphic
			{
				get { return (TextAreaGraphic) base.StatefulGraphic; }
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				UserTextGraphic userTextGraphic = (UserTextGraphic)this.StatefulGraphic.InteractiveGraphic;

				this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					RectangleF boundingBox = userTextGraphic.BoundingBox;

					if (mouseInformation.ClickCount == 2
					    && boundingBox.Contains(mouseInformation.Location))
					{
						// double click action on the callout text: send into edit text mode
						userTextGraphic.StartEdit();
					}
				}
				finally
				{
					this.StatefulGraphic.ResetCoordinateSystem();
				}

				if (base.Start(mouseInformation))
					return true;

				return false;
			}
		}
	}
}