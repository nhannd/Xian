using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class TextEditControlGraphic : ControlGraphic, ITextGraphic, IMemorable
	{
		private bool _multiline = true;

		private bool _deleteOnEmpty = false;

		[CloneIgnore]
		private EditBox _currentCalloutEditBox;

		public TextEditControlGraphic(IGraphic subject) : base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ITextGraphic));
		}

		protected TextEditControlGraphic(TextEditControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new ITextGraphic Subject
		{
			get { return base.Subject as ITextGraphic; }
		}

		public override string CommandName
		{
			get { return SR.CommandEditText; }
		}

		public bool Multiline
		{
			get { return _multiline; }
			set { _multiline = value; }
		}

		public bool DeleteOnEmpty
		{
			get { return _deleteOnEmpty; }
			set { _deleteOnEmpty = value; }
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
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(this.Text ?? string.Empty);
				editBox.Location = Point.Round(this.Location);
				editBox.Size = Rectangle.Round(this.BoundingBox).Size;
				editBox.Multiline = this.Multiline;
				editBox.FontName = this.FontName;
				editBox.FontSize = this.FontSize;
				editBox.ValueAccepted += OnEditBoxComplete;
				editBox.ValueCancelled += OnEditBoxComplete;
				InstallEditBox(_currentCalloutEditBox = editBox);
				result = true;
			}
			finally
			{
				this.ResetCoordinateSystem();
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
			if (base.ParentPresentationImage != null)
			{
				if (base.ParentPresentationImage.Tile != null)
					base.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e)
		{
			bool removeFromParent = false;
			if (_currentCalloutEditBox != null)
			{
				removeFromParent = _deleteOnEmpty && string.IsNullOrEmpty(_currentCalloutEditBox.LastAcceptedValue);
				if (!removeFromParent)
				{
					object state = this.CreateMemento();
					this.Text = _currentCalloutEditBox.LastAcceptedValue;
					this.AddToCommandHistory(state, this.CreateMemento());
					this.Draw();
				}
			}
			EndEdit();

			if (removeFromParent && base.ImageViewer != null)
			{
				// find the highest-level control graphic
				IGraphic graphic = this;
				while (graphic != null && graphic.ParentGraphic is IControlGraphic)
					graphic = graphic.ParentGraphic;

				if (graphic != null && graphic.ParentPresentationImage != null)
				{
					IImageViewer imageViewer = base.ImageViewer;
					DrawableUndoableCommand command = new DrawableUndoableCommand(graphic.ParentPresentationImage);
					command.Name = SR.CommandDelete;
					command.Enqueue(new RemoveGraphicUndoableCommand(graphic));
					command.Execute();
					imageViewer.CommandHistory.AddCommand(command);
				}
			}
		}

		private void AddToCommandHistory(object beginState, object endState)
		{
			if (beginState != null && endState != null && !beginState.Equals(endState) && base.ImageViewer != null)
			{
				MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(this);
				memorableCommand.BeginState = beginState;
				memorableCommand.EndState = endState;

				DrawableUndoableCommand command = new DrawableUndoableCommand(this);
				command.Name = this.CommandName;
				command.Enqueue(memorableCommand);

				base.ImageViewer.CommandHistory.AddCommand(command);
			}
		}

		protected override IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			IResourceResolver resolver = new ResourceResolver(this.GetType(), true);
			string @namespace = typeof(TextEditControlGraphic).FullName;
			MenuAction action = new MenuAction(@namespace + ":edit", new ActionPath(site + "/MenuEditText", resolver), ClickActionFlags.None, resolver);
			action.GroupHint = new GroupHint("Tools.Graphics.Edit");
			action.Label = SR.MenuEditText;
			action.Persistent = true;
			action.SetClickHandler(delegate { this.StartEdit(); });
			return new ActionSet(new IAction[] {action});
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ClickCount == 2 && mouseInformation.ActiveButton == XMouseButtons.Left)
			{
				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (this.HitTest(mouseInformation.Location))
					{
						this.StartEdit();
						return true;
					}
					else
					{
						this.EndEdit();
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}
			return base.OnMouseStart(mouseInformation);
		}

		#region ITextGraphic Members

		public float FontSize
		{
			get { return this.Subject.SizeInPoints; }
			set { this.Subject.SizeInPoints = value; }
		}

		public string FontName
		{
			get { return this.Subject.Font; }
			set { this.Subject.Font = value; }
		}

		public string Text
		{
			get { return this.Subject.Text; }
			set { this.Subject.Text = value; }
		}

		float ITextGraphic.SizeInPoints
		{
			get { return this.FontSize; }
			set { this.FontSize = value; }
		}

		string ITextGraphic.Font
		{
			get { return this.FontName; }
			set { this.FontName = value; }
		}

		public PointF Location
		{
			get { return this.Subject.Location; }
			set { this.Subject.Location = value; }
		}

		public SizeF Dimensions
		{
			get { return this.Subject.Dimensions; }
		}

		public LineStyle LineStyle
		{
			get { return this.Subject.LineStyle; }
			set { this.Subject.LineStyle = value; }
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento()
		{
			return this.Subject.Text;
		}

		public void SetMemento(object memento)
		{
			if (!(memento is string))
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.Text = (string) memento;
		}

		#endregion
	}
}