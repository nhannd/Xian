using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A subclass of <see cref="UndoableCommand"/> for <see cref="IDrawable"/> objects.
	/// </summary>
	/// <remarks>
	/// Often, when an <see cref="UndoableCommand"/> is <see cref="Execute"/>d or
	/// <see cref="Unexecute"/>d, it is necessary to refresh an <see cref="IDrawable"/>
	/// object, such as an <see cref="ITile"/>.  This class automatically calls
	/// <see cref="IDrawable.Draw"/> on the object passed to the constructor
	/// after <see cref="Execute"/> and <see cref="Unexecute"/>.
	/// </remarks>
	public class DrawableUndoableCommand : UndoableCommand
	{
		private IDrawable _drawable;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="drawable">The object to redraw after <see cref="Execute"/> or <see cref="Unexecute"/>.</param>
		/// <param name="originator">The originator is the object responsible for creating
		/// memento objects and restoring state from them.</param>
		public DrawableUndoableCommand(IDrawable drawable, IMemorable originator)
			: base(originator)
		{
			Platform.CheckForNullReference(drawable, "drawable");
			Platform.CheckForNullReference(originator, "originator");

			_drawable = drawable;
		}

		/// <summary>
		/// Default constructor for subclasses.
		/// </summary>
		/// <remarks>
		/// Subclasses that use this constructor <b>must</b> override
		/// <see cref="Execute"/> and <see cref="Unexecute"/>, otherwise
		/// the command will do nothing (<see cref="UndoableCommand.Originator"/> will be null).
		/// </remarks>
		protected DrawableUndoableCommand(IDrawable drawable)
			: base()
		{
			Platform.CheckForNullReference(drawable, "drawable");
			_drawable = drawable;
		}

		/// <summary>
		/// Gets the <see cref="IDrawable"/> object that will be redrawn on <see cref="Unexecute"/>
		/// or <see cref="Execute"/>.
		/// </summary>
		protected IDrawable Drawable
		{
			get { return _drawable; }	
		}

		/// <summary>
		/// Performs a 'redo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Execute()
		{
			base.Execute();
			_drawable.Draw();
		}

		/// <summary>
		/// Performs an 'undo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Unexecute()
		{
			base.Unexecute();
			_drawable.Draw();
		}
	}
}
